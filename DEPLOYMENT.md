# Azure-Bereitstellung

Diese Anleitung beschreibt Schritt für Schritt, wie die Anwendung mit **App Service**,
**Azure SQL Database** und **Azure Storage** im Azure-Portal bereitgestellt wird.

## Überblick

| Komponente        | Azure-Dienst                  | Zweck                                  |
|-------------------|-------------------------------|----------------------------------------|
| Web-App           | App Service (Linux, .NET 10)  | Hostet die ASP.NET-Anwendung           |
| Datenbank         | Azure SQL Database            | Lehrveranstaltungen, Kapitel, Fragen … |
| Datei-Upload      | Storage Account (Blob)        | Hochgeladene PDF-Foliensätze           |
| KI                | Google Gemini API (extern)    | Fragen generieren & prüfen             |

Die Anwendung ist provider-unabhängig vorbereitet: Lokal läuft sie mit **SQLite**, in Azure
mit **SQL Server**. Gesteuert wird das über die App-Einstellung `Database__Provider`.
Beim ersten Start legt die App das Datenbankschema inklusive Demo-Daten automatisch an
(`EnsureCreated` für SQL Server, EF-Migrationen für SQLite).

## Voraussetzungen

- Azure-Abonnement (Studierende: kostenloses Azure-Kontingent möglich)
- Anwendung lässt sich lokal bauen: `dotnet build`
- Google-Gemini-API-Schlüssel (https://aistudio.google.com/apikey)

---

## Schritt 1 – Ressourcengruppe anlegen

1. Azure-Portal → **Ressourcengruppen** → **Erstellen**.
2. Name z. B. `rg-mc-klausurmanager`, Region z. B. *West Europe*. **Überprüfen + Erstellen**.

## Schritt 2 – Azure SQL Database

1. Portal → **SQL-Datenbanken** → **Erstellen**.
2. Ressourcengruppe wählen; Datenbankname z. B. `mc-klausurmanager-db`.
3. **Server** → *Neu erstellen*: Servername (global eindeutig), Region, **SQL-Authentifizierung**
   mit Admin-Login und Passwort (notieren!).
4. Compute + Storage: für die Demo genügt **Basic** bzw. das kostenlose Serverless-Kontingent.
5. Netzwerk → **Verbindungsmethode: Öffentlicher Endpunkt**; beide Schalter aktivieren:
   - *Azure-Diensten und -Ressourcen den Zugriff auf diesen Server erlauben*
   - *Aktuelle Client-IP-Adresse hinzufügen* (für eigene Tests)
6. **Erstellen**. Danach in der Datenbank unter **Verbindungszeichenfolgen → ADO.NET** den
   Connection-String kopieren und `{your_password}` durch das Passwort ersetzen.

## Schritt 3 – Storage Account für PDF-Uploads

1. Portal → **Speicherkonten** → **Erstellen**. Name (global eindeutig, klein), Region,
   Leistung *Standard*, Redundanz *LRS* genügt.
2. Nach der Erstellung: **Container** → **+ Container** → Name `slides`. Zugriffsebene:
   *Privat* (die App liefert die Datei-URLs aus) – oder *Blob*, falls die Links ohne
   Authentifizierung direkt im Browser geöffnet werden sollen.
3. **Sicherheit + Netzwerk → Zugriffsschlüssel** → **Verbindungszeichenfolge** kopieren.

## Schritt 4 – App Service (Web-App)

1. Portal → **App Services** → **Erstellen → Web-App**.
2. Ressourcengruppe wählen; Name (= URL `https://<name>.azurewebsites.net`).
3. **Veröffentlichen: Code**, **Laufzeitstapel: .NET 10**, **Betriebssystem: Linux**, Region.
4. App Service-Plan: für die Demo z. B. **B1** (Basic) oder Free **F1**.
5. **Überprüfen + Erstellen**.

## Schritt 5 – App-Einstellungen konfigurieren

App Service → **Einstellungen → Umgebungsvariablen → App-Einstellungen**. Folgende Einträge
hinzufügen (Namen exakt so – der doppelte Unterstrich `__` bildet die JSON-Verschachtelung ab):

| Name                                  | Wert                                                            |
|---------------------------------------|----------------------------------------------------------------|
| `ASPNETCORE_ENVIRONMENT`              | `Production`                                                    |
| `Database__Provider`                  | `SqlServer`                                                     |
| `ConnectionStrings__DefaultConnection`| *ADO.NET-Verbindungsstring aus Schritt 2 (mit Passwort)*        |
| `AzureBlobStorage__ConnectionString`  | *Storage-Verbindungsstring aus Schritt 3*                       |
| `AzureBlobStorage__ContainerName`     | `slides`                                                        |
| `Gemini__ApiKey`                      | *dein Gemini-API-Schlüssel*                                     |

Anschließend **Speichern** (die App startet neu).

> Hinweis: Alternativ kann der DB-String unter **Verbindungszeichenfolgen** als Typ
> *SQLAzure* mit Namen `DefaultConnection` hinterlegt werden – ASP.NET liest beide Varianten.
> Den Storage- und Gemini-Schlüssel idealerweise über **Azure Key Vault** referenzieren.

## Schritt 6 – Anwendung veröffentlichen

Eine der folgenden Methoden:

- **Rider:** Rechtsklick auf das Projekt → *Publish* → *Azure* → die erstellte Web-App wählen.
- **Visual Studio:** *Veröffentlichen* → *Azure* → *Azure App Service (Linux)*.
- **az CLI (ZIP-Deploy):**
  ```bash
  dotnet publish -c Release -o ./publish
  cd publish && zip -r ../app.zip . && cd ..
  az webapp deploy --resource-group rg-mc-klausurmanager \
    --name <app-name> --src-path app.zip --type zip
  ```
- **GitHub Actions:** Im App Service unter *Bereitstellungscenter* GitHub verbinden – Azure
  erzeugt automatisch einen Workflow.

## Schritt 7 – Verifizieren

1. `https://<app-name>.azurewebsites.net` öffnen → Startseite mit Hero erscheint.
2. Beim ersten Aufruf legt die App Schema + Demo-Daten in Azure SQL an. Unter
   **Lehrveranstaltungen** sollten die Beispieldaten inklusive der Demo-Prüfung sichtbar sein.
3. Ein Kapitel öffnen, eine PDF hochladen → die Datei landet im Container `slides`.
4. „KI-Frage generieren" / „KI-Prüfung" testen (benötigt gültigen `Gemini__ApiKey`).

---

## Hinweise & Troubleshooting

- **Datenbank-Schema:** In Azure wird das Schema per `EnsureCreated()` aus dem Modell erzeugt.
  Spätere Schemaänderungen werden damit **nicht** automatisch nachgezogen – für ein produktives
  Versionieren des Schemas auf SQL Server müssten SqlServer-Migrationen ergänzt werden.
- **Verbindung schlägt fehl:** Prüfen, ob in der SQL-Server-Firewall *„Azure-Diensten Zugriff
  erlauben"* aktiviert ist und der Connection-String das korrekte Passwort enthält.
- **Datei-Download 404 / kein Zugriff:** Container-Zugriffsebene auf *Blob* setzen oder die
  Auslieferung über SAS-Token ergänzen.
- **Sicherheitswarnung NU1903** (`SQLitePCLRaw.lib.e_sqlite3 2.1.10`): stammt aus dem
  SQLite-Provider und betrifft nur die lokale Entwicklung; bei Bedarf das EF-Sqlite-Paket
  aktualisieren.
- **Kosten:** Web-App (F1/B1), Azure SQL (Basic/Serverless) und Storage (LRS) sind im
  kleinsten Tarif sehr günstig bzw. teils kostenlos; nach der Demo die Ressourcengruppe löschen.

# MC-Klausurmanager

Webanwendung zur Verwaltung von **Multiple-Choice-Klausuren** an einem Lehrstuhl.
Sie unterstützt das Anlegen von Lehrveranstaltungen und Kapiteln (inkl. PDF-Foliensätzen),
das Pflegen eines Fragenkatalogs sowie das zufällige Zusammenstellen und Ausdrucken von
Prüfungen. Über die Google-Gemini-API können Fragen aus den hochgeladenen Folien automatisch
generiert und auf sprachliche Qualität geprüft werden.

Gruppenprojekt im Modul *Web Engineering*, Universität Würzburg.

## Tech-Stack

- **ASP.NET Core MVC** (.NET 10)
- **Entity Framework Core** (SQLite lokal, SQL Server / Azure SQL in der Cloud)
- **Bootstrap 5** für die Oberfläche
- **Azure Blob Storage** für hochgeladene PDF-Foliensätze (lokal: Dateiablage im `wwwroot`)
- **Google Gemini** als LLM zum Generieren und Prüfen von Fragen

## Voraussetzungen

- **.NET SDK 10** ([Download](https://dotnet.microsoft.com/download))

Für die lokale Entwicklung sind keine weiteren Dienste nötig: Es wird eine lokale
SQLite-Datei verwendet, und Datei-Uploads werden im Dateisystem abgelegt.

## Lokales Setup

```bash
# 1. Repository klonen
git clone <repository-url>
cd Web_Engineering_Gruppenprojekt

# 2. Abhängigkeiten wiederherstellen
dotnet restore

# 3. Anwendung starten
dotnet run --project Web_Engineering_Gruppenprojekt
```

Die **Datenbank wird beim ersten Start automatisch initialisiert**: Für SQLite werden die
EF-Core-Migrationen angewendet und Demo-Daten (Beispiel-Lehrveranstaltungen, Kapitel, Fragen
und eine Demo-Prüfung) angelegt. Ein manueller Migrationsschritt ist nicht erforderlich.

Anschließend ist die Anwendung erreichbar unter:

- https://localhost:7011
- http://localhost:5178

## Konfiguration

Die Anwendung liest folgende Werte aus der Konfiguration (`appsettings.json`,
Umgebungsvariablen bzw. App-Einstellungen):

| Schlüssel                              | Zweck                                                |
|----------------------------------------|------------------------------------------------------|
| `ConnectionStrings:DefaultConnection`  | Datenbank-Verbindungsstring                          |
| `Database:Provider`                    | `Sqlite` (Standard, lokal) oder `SqlServer` (Azure)  |
| `AzureBlobStorage:ConnectionString`    | Verbindungsstring für den Blob-Storage (nur Cloud)   |
| `AzureBlobStorage:ContainerName`       | Blob-Container für PDFs (z. B. `slides`)              |
| `Gemini:ApiKey`                        | API-Schlüssel für Google Gemini                      |

> **Wichtig:** Der Gemini-API-Schlüssel und produktive Connection-Strings sind **Geheimnisse**
> und dürfen **nicht ins Repository eingecheckt** werden. Für die lokale Entwicklung eignen
> sich [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
> (`dotnet user-secrets`) oder Umgebungsvariablen; in Azure werden die Werte über die
> App-Einstellungen (idealerweise via Key Vault) hinterlegt. Die im Repository enthaltenen
> Konfigurationsdateien (`appsettings.json` und `appsettings.Development.json`) lassen diese
> Felder bewusst leer.

## Deployment

Eine ausführliche Schritt-für-Schritt-Anleitung für die Bereitstellung auf Azure
(App Service, Azure SQL Database, Blob Storage) findet sich in **[DEPLOYMENT.md](DEPLOYMENT.md)**.

## Projektstruktur

```
Web_Engineering_Gruppenprojekt/
├── Controllers/    MVC-Controller
├── Data/           AppDbContext und Seed-Daten
├── Migrations/     EF-Core-Migrationen (SQLite)
├── Models/         Datenmodell (Course, Chapter, MCQuestion, …)
├── Services/       Datei-Storage- und Gemini-Service
├── Views/          Razor-Views (Bootstrap 5)
└── wwwroot/        Statische Dateien, lokale PDF-Uploads
```

Eine Übersicht der eingesetzten KI-Prompts dokumentiert **[PROMPTS.md](PROMPTS.md)**.

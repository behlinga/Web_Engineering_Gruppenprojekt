# Verwendete KI-Prompts

Dieses Dokument fasst die für die Umsetzung des Gruppenprojekts verwendeten Prompts zusammen.
Statt vieler kleiner Einzelprompts ist je Feature ein größerer, strukturierter Prompt
dokumentiert (wie im Lastenheft-Tipp empfohlen). Als KI-Werkzeug wurde ein agentischer
Coding-Assistent (Claude / Claude Code) eingesetzt, der Dateien direkt im Projekt anlegt,
den Build ausführt und die Anwendung zum Testen startet.

**Technischer Rahmen (in jedem Prompt als Kontext vorausgesetzt):**
ASP.NET Core MVC, .NET 10, Entity Framework Core, Bootstrap 5, SQLite (lokal) bzw.
Azure SQL (Cloud), Azure Blob Storage für Datei-Uploads, Google Gemini als LLM.

---

## Prompt 0 – Projektkontext & Architektur

> Wir entwickeln eine ASP.NET-Core-MVC-Anwendung (.NET 10), die einen Lehrstuhl bei der
> Erstellung von Multiple-Choice-Klausuren unterstützt. Verwende Entity Framework Core als
> ORM, Bootstrap 5 für die Oberfläche und eine saubere Schichtung (Models, Controllers,
> Views, Services). Richte das Projekt so ein, dass es lokal mit SQLite läuft und später
> auf Azure (App Service + Azure SQL + Blob Storage) bereitgestellt werden kann. Halte den
> Code idiomatisch (Primary Constructors, async/await, Nullable-Reference-Types).

---

## Prompt 1 – Datenmodell, DbContext und Beispieldaten

> Erstelle das Datenmodell mit folgenden Entitäten und Beziehungen:
> - **Course** (Lehrveranstaltung): Titel, Dozentenname, Niveau (Enum Bachelor/Master).
> - **Chapter** (Kapitel): Titel, Kapitelnummer, optionaler Dateiname + Pfad der Folien;
>   gehört zu genau einer Course.
> - **MCQuestion**: Fragentext; gehört zu genau einem Chapter.
> - **MCAnswerOption**: Antworttext, IsCorrect (bool); gehört zu genau einer MCQuestion.
> - **Exam** (Prüfung): Datum; gehört zu genau einer Course.
> - **ExamQuestion**: m:n-Verknüpfung zwischen Exam und MCQuestion (zusammengesetzter PK).
>
> Konfiguriere im AppDbContext kaskadierendes Löschen entlang Course → Chapter → Question →
> Option sowie Course → Exam → ExamQuestion, sodass beim Löschen abhängige Datensätze
> mitentfernt werden. Lege Beispieldaten für Demozwecke an (mehrere Lehrveranstaltungen,
> Kapitel, Fragen mit je vier Antwortoptionen und einer korrekten Antwort sowie eine
> Beispiel-Prüfung). Erzeuge eine EF-Migration.

---

## Prompt 2 – Datei-Storage abstrahieren (lokal + Azure Blob)

> Erstelle eine Abstraktion `IFileStorageService` mit `UploadAsync(IFormFile)`,
> `DeleteAsync(path)` und `GetUrl(path)`. Implementiere zwei Varianten:
> 1. `LocalFileStorageService` – speichert PDFs im wwwroot (für die lokale Entwicklung).
> 2. `AzureBlobStorageService` – speichert die Dateien in einem Azure-Blob-Container.
>
> Registriere im DI-Container abhängig von Umgebung/Konfiguration automatisch die passende
> Implementierung (Azure in Produktion, sofern ein Connection-String vorhanden ist, sonst
> lokal). Es sollen ausschließlich PDF-Dateien hochgeladen werden.

---

## Prompt 3 – KI-Service (Gemini): Fragen generieren und prüfen

> Erstelle einen `IGeminiService` mit zwei Funktionen, der die Google-Gemini-API per
> HttpClient anspricht (API-Key aus der Konfiguration):
> 1. `GenerateQuestionAsync(pdfPath, chapterId)` – lädt die zum Kapitel gehörende PDF als
>    Inline-Daten an das Modell, fordert eine neue MC-Frage mit vier Optionen und genau
>    einer korrekten Antwort an (Antwort als striktes JSON), parst sie robust und speichert
>    die Frage samt Optionen in der Datenbank.
> 2. `CheckQuestionAsync(questionId)` – schickt Frage und Antwortoptionen an das Modell mit
>    der Aufgabe, sprachliche Qualität und Eindeutigkeit zu bewerten (genau eine korrekte
>    Antwort), und gibt eine kurze deutschsprachige Bewertung zurück.
>
> Behandle fehlenden API-Key und Fehlerfälle defensiv (verständliche Rückgabewerte statt
> Exceptions).

---

## Prompt 4 – CRUD für Lehrveranstaltungen und Kapitel inkl. PDF-Upload

> Implementiere Controller und Bootstrap-Views für die Verwaltung (Anlegen, Bearbeiten,
> Löschen) von Lehrveranstaltungen und Kapiteln:
> - **Lehrveranstaltungsliste**: nach Titel sortiert; je Eintrag Badge-Buttons mit der
>   Anzahl der Kapitel und der Prüfungen, die zur jeweiligen Detailliste verlinken.
> - **Kapitelliste** einer Lehrveranstaltung: oberhalb die Stammdaten der Veranstaltung,
>   Liste nach Kapitelnummer sortiert. Ist eine Datei hochgeladen, zeige einen Link darauf;
>   im Bearbeitungsmodus kann die Datei gelöscht werden, andernfalls erscheint ein
>   File-Upload-Feld. Je Kapitel ein Badge-Button mit der Anzahl der Fragen.
>
> Beim Upload/Löschen die `IFileStorageService`-Abstraktion verwenden. Beim Löschen werden
> abhängige Datensätze (und zugehörige Dateien) mitentfernt.

---

## Prompt 5 – Fragenkatalog: Anzeige, Bearbeitung und KI-Funktionen

> Implementiere die Fragenansicht je Kapitel:
> - Jede Frage als Bootstrap-List-Group: zuoberst der Fragentext, darunter die vier
>   Antwortoptionen, die korrekte Option klar erkennbar hervorgehoben; im letzten Item
>   Buttons zum Bearbeiten, Löschen und „KI-Prüfung".
> - Das **Bearbeiten** erfolgt in einer separaten View mit einem ViewModel; die vier
>   Optionen haben je ein Radio-Control zur Auswahl der korrekten Antwort.
> - Der **KI-Prüfen-Button** ruft per AJAX den Server auf, der `CheckQuestionAsync` nutzt;
>   das Ergebnis wird in einem Bootstrap-Modal angezeigt.
> - Oberhalb der Liste ein Button **„KI-Frage generieren"**, der – sofern eine PDF
>   hochgeladen ist – über `GenerateQuestionAsync` eine neue Frage erzeugt und der Liste
>   hinzufügt.

---

## Prompt 6 – Prüfungen zusammenstellen und drucken

> Implementiere die Prüfungsverwaltung je Lehrveranstaltung:
> - **Prüfungsliste** mit Button „Neue Prüfung erstellen". Beim Erstellen wird eine frei
>   wählbare Anzahl n an Fragen **zufällig und ohne Dubletten** aus allen Fragen der Kapitel
>   dieser Lehrveranstaltung ausgewählt und mit der Prüfung verknüpft. Validierung, falls n
>   größer als der verfügbare Fragenbestand ist.
> - Jeder Prüfungseintrag verlinkt auf eine **druckbare Ansicht** (eigenes Layout) der
>   zugehörigen Fragen samt Antwortoptionen, geeignet zum Ausdrucken der Klausur.

---

## Prompt 7 – Startseite, Navigation und Layout

> Gestalte die Startseite mit Bootstrap: eine Menüleiste (Navbar) mit Markenname und einem
> Link zu den Lehrveranstaltungen sowie darunter ein „Hero"-Bereich mit Überschrift und
> kurzem Beschreibungstext der Anwendung und einem Call-to-Action-Button zur
> Lehrveranstaltungsliste. Ergänze darunter kurze Feature-Karten. Verwende durchgängig
> Bootstrap-Komponenten (Tabellen, Badges, Buttons, Breadcrumbs, Cards, Modals).

---

## Prompt 8 – Azure-Bereitstellung vorbereiten

> Mache die Anwendung Azure-fähig, ohne die lokale SQLite-Entwicklung zu beeinträchtigen:
> - Mache den Datenbankprovider über die Konfiguration (`Database:Provider`) umschaltbar:
>   SQLite lokal (EF-Migrationen), SQL Server in Azure. Beim Start initialisiert die App die
>   Datenbank automatisch – für SQL Server via `EnsureCreated` (Schema + Seed-Daten direkt
>   aus dem Modell), für SQLite via Migrationen.
> - Lies Connection-Strings, den Blob-Storage-Connection-String und den Gemini-API-Key aus
>   der Konfiguration / den App-Einstellungen.
> - Erstelle eine Schritt-für-Schritt-Anleitung (DEPLOYMENT.md) für die Bereitstellung im
>   Azure-Portal: Ressourcengruppe, Azure SQL Database, Storage Account (Container
>   `slides`), App Service (.NET 10), zu setzende App-Einstellungen, Deployment und
>   Verifizierung.

---

## Prompt 9 – Bugfixes, fehlende Features und Qualitätsverbesserungen

> Teste die App systematisch gegen das Lastenheft und behebe alle gefundenen Abweichungen:
>
> 1. **Bug: Löschen von Fragen schlägt fehl**, wenn die Frage einer Prüfung zugeordnet ist.
>    Ursache: `DeleteBehavior.Restrict` auf `ExamQuestion → MCQuestion`. Fix: In der
>    `Delete`-Action die `ExamQuestions` per `Include` mitladen und vor dem Löschen der
>    Frage explizit mit `RemoveRange` entfernen.
>
> 2. **Fehlende Funktion: Fragen manuell anlegen.** Das Lastenheft fordert CRUD für Fragen.
>    Ergänze `Create` (GET + POST) im `MCQuestionController` und eine eigene View
>    `MCQuestion/Create.cshtml` (analog zur Edit-View, mit Breadcrumb und 4 Optionen per
>    Radio-Control). Füge in der Fragenliste einen „+ Frage manuell"-Button hinzu.
>
> 3. **Sicherheit: `[ValidateAntiForgeryToken]` für `CheckQuestion`-Endpoint** fehlte. Füge
>    das Attribut hinzu und sende das CSRF-Token im AJAX-Fetch als `FormData`-Body statt als
>    Header (da ASP.NET den Token standardmäßig im Request-Body erwartet).
>
> 4. **Druckansicht: Lösungsblatt für Korrektoren.** Ergänze in `ExamController.Print` einen
>    optionalen Parameter `bool showAnswers = false`. In `Print.cshtml` werden bei
>    `showAnswers=true` die korrekten Antworten grün markiert (`.correct-answer`) und mit
>    Häkchen versehen; ein Umschalter-Button wechselt zwischen Studenten- und
>    Korrektorenversion.
>
> 5. **UX: Erfolgsmeldung nach KI-Fragen-Generierung.** Setze `TempData["Success"]` wenn
>    `GenerateQuestionAsync` erfolgreich war; zeige den Alert in der Fragenliste an.
>
> 6. **UX: Kaskadierungs-Info auf Delete-Seiten.** Beim Löschen einer Lehrveranstaltung die
>    Anzahl betroffener Kapitel, Fragen und Prüfungen anzeigen; beim Löschen eines Kapitels
>    die Anzahl betroffener Fragen. Dazu je eine zusätzliche DB-Abfrage im Controller und
>    `ViewBag`-Übergabe an die View.
>
> 7. **Prüfungsliste: Datum als Link auf Druckansicht.** Das Lastenheft schreibt vor, dass
>    „jeder Eintrag ein Link auf die druckbare Liste" ist. Das Datum-`<td>` in
>    `Exam/Index.cshtml` muss als `<a asp-action="Print">` formatiert sein.
>
> 8. **Code-Qualität: `Random.Shared.Shuffle` statt `OrderBy(_ => Guid.NewGuid())`.** Das
>    Guid-Shuffle ist kein zuverlässiges Zufallsverfahren. Konvertiere die Liste in ein Array
>    und nutze `Random.Shared.Shuffle(arr)` (verfügbar ab .NET 8).

---

## Prompt 10 – Live-Fehleranalyse und Behebung (Azure-Deployment)

> Teste die auf Azure App Service deployte Anwendung systematisch (Klick-Test aller Views)
> und verifiziere jeden gefundenen Fehler zusätzlich im Quellcode, bevor du ihn behebst.
> Behebe anschließend folgende Punkte:
>
> 1. **Bug: KI-Prüfung lädt endlos.** In `MCQuestion/Index.cshtml` überschreibt im
>    `checkQuestion(id)`-Script eine innere `const body = new FormData()` die äußere
>    `const body`-Variable (das Modal-Div) durch Shadowing. Dadurch schreibt
>    `body.innerText = data.feedback` ins FormData-Objekt statt ins DOM, der Spinner bleibt
>    hängen. Fix: innere Variable in `formData` umbenennen.
>
> 2. **Fehlende Klickbarkeit: Fragen-Badge in der Prüfungsliste.** Analog zum Datum soll auch
>    der „X Fragen“-Badge in `Exam/Index.cshtml` als `<a asp-action="Print">` zur Druckansicht
>    verlinken statt ein reines `<span>` zu sein.
>
> 3. **Unnötigen Privacy-Link entfernen.** In `Shared/_Layout.cshtml` verweist der
>    Footer-Link auf eine nicht existierende bzw. nicht geforderte Privacy-Seite – entfernen.
>
> 4. **UI: Bearbeiten/Löschen als Icons statt Text.** In `Course/Index.cshtml`,
>    `Chapter/Index.cshtml`, `MCQuestion/Index.cshtml` und `Exam/Index.cshtml` die
>    Text-Buttons „Bearbeiten“/„Löschen“ durch Bootstrap-Icons (`bi-pencil`, `bi-trash`) mit
>    `title`-Attribut für Barrierefreiheit ersetzen. Dazu Bootstrap Icons per CDN im Layout
>    einbinden.
>
> 5. **Bug: Zurück-Button in der Druckansicht funktioniert nicht / neue Tabs.** In
>    `Exam/Index.cshtml` öffnen Prüfungs- und Drucken-Links mit `target="_blank"` einen neuen
>    Tab ohne Historie; der Zurück-Link in `Exam/Print.cshtml` nutzt
>    `javascript:history.back()` und läuft dort ins Leere. Fix: `target="_blank"` entfernen
>    und den Zurück-Link stattdessen auf eine echte URL
>    (`Url.Action("Index", "Exam", new { courseId = Model.CourseId })`) setzen.
>
> 6. **Schwerwiegender Bug: Kapitel/Kurs löschen kann eine Fremdschlüssel-Exception werfen.**
>    `ExamQuestion → MCQuestion` ist auf `DeleteBehavior.Restrict` konfiguriert;
>    `ChapterController.DeleteConfirmed` und `CourseController.DeleteConfirmed` entfernen die
>    zugehörigen `ExamQuestion`-Verknüpfungen nicht (im Gegensatz zur bereits korrekten
>    `MCQuestionController.Delete`). Fix: In beiden Delete-Actions vor dem eigentlichen
>    Löschen alle `ExamQuestion`-Einträge der betroffenen Fragen per `RemoveRange` entfernen
>    (beim Kapitel über `Include(Questions).ThenInclude(ExamQuestions)`, beim Kurs über eine
>    Query über alle Kapitel/Fragen des Kurses).
>
> 7. **Bug: Namensfeld verschwindet beim Drucken.** In `Exam/Print.cshtml` trägt die
>    Name-/Matrikelnummer-Tabelle fälschlich die Klasse `no-print` und wird beim Drucken
>    ausgeblendet – Klasse entfernen (nur die Buttonleiste soll `no-print` bleiben).
>
> 8. **Bug: PDF nicht abrufbar (`PublicAccessNotPermitted`) und KI-Fragengenerierung ignoriert
>    das PDF – gemeinsame Wurzel: kein authentifizierter Blob-Zugriff.**
>    `AzureBlobStorageService` speichert und liefert die nackte Blob-URI ohne SAS-Token, der
>    Storage Account erlaubt aber keinen anonymen Zugriff; `GeminiService` liest PDFs nur vom
>    lokalen Dateisystem und überspringt http(s)-Pfade (Azure Blob) komplett. Fix:
>    - Ergänze `IFileStorageService` um `DownloadAsync(path)` für authentifizierten
>      Byte-Zugriff; implementiere es in `LocalFileStorageService` (Datei aus `wwwroot`
>      lesen) und in `AzureBlobStorageService` (`BlobClient.DownloadContentAsync()`).
>    - `AzureBlobStorageService.GetUrl(path)` generiert per `BlobClient.GenerateSasUri`
>      (Read, 1h Gültigkeit) eine signierte URL statt die rohe Blob-URI zurückzugeben.
>    - `GeminiService.GenerateQuestionAsync` nutzt statt direktem Dateisystemzugriff nur
>      noch `IFileStorageService.DownloadAsync`, wodurch lokale und Azure-Pfade einheitlich
>      und transparent funktionieren (kein Sonderfall mehr für http-Präfixe nötig).
>    - Views (`Chapter/Index.cshtml`, `Chapter/Edit.cshtml`) verlinken die Folien-URL über
>      `FileStorage.GetUrl(...)` statt die gespeicherte `SlidePath` direkt zu verwenden.
>
> 9. **Robustheit: Kein Timeout beim Gemini-HttpClient.** Ergänze in `Program.cs`
>    `.ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(30))` beim Registrieren von
>    `IGeminiService`, damit eine hängende Gemini-API den Request nicht unbegrenzt blockiert.
>
> 10. **UX: Kein Lade-Feedback bei „KI-Frage generieren“.** Der Button löst einen normalen
>     Formular-POST aus, die Seite wirkt bis zum Neuladen eingefroren. Deaktiviere den Button
>     per JavaScript beim Submit und zeige einen Bootstrap-Spinner mit „Wird generiert...“.

---

## Prompt 11 – Cold-Start-Robustheit und weniger repetitive KI-Fragen

> Behebe drei Punkte, ohne bestehende Funktionalität (KI-Prüfung, Blob-Upload,
> Prüfungserstellung) zu verändern:
>
> 1. **Bug: Intermittierende Fehlerseiten beim ersten Request (Cold Start).** Nach
>    Inaktivität wirft der erste Request eine Fehlerseite, weil die Azure-SQL-Verbindung
>    beim Cold Start noch nicht steht. Fix: Im SqlServer-Zweig von
>    `AddDbContext<AppDbContext>` Connection Resiliency aktivieren
>    (`options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(...))`, der
>    Sqlite-Zweig bleibt unverändert). Zusätzlich den DB-Init-Block (`EnsureCreated`/
>    `Migrate`) mit einer kleinen Retry-Schleife (3 Versuche, kurzer Delay) umschließen,
>    damit ein fehlgeschlagener erster Verbindungsaufbau die App nicht crasht. Als Kommentar
>    ergänzen, dass zusätzlich „Always On“ im App Service aktiviert werden sollte (kann nicht
>    per Code gesetzt werden).
>
> 2. **Verifikation: Hero-Button auf der Startseite.** Prüfe, ob der Button „Zu den
>    Lehrveranstaltungen“ ein sauberer Link ohne dazwischenfunkenden JS-Handler ist. Ergebnis:
>    `wwwroot/js/site.js` ist leer, kein `addEventListener`/`preventDefault` im Projekt – der
>    `<a asp-controller="Course" asp-action="Index">` löst korrekt auf, keine Änderung nötig.
>
> 3. **Problem: KI generiert fast identische Fragen.** `GenerateQuestionAsync` bekommt nur
>    das PDF und den Auftrag „Erstelle eine Frage“ – ohne Kenntnis existierender Fragen, was
>    bei kurzen PDFs zu Wiederholungen führt. Fix: Vor dem Prompt-Aufbau die vorhandenen
>    Fragen des Kapitels laden (`db.Questions.Where(q => q.ChapterId == chapterId)
>    .Select(q => q.QuestionText).ToListAsync()`) und als Liste in den Prompt einbetten, mit
>    der expliziten Anweisung, ein anderes Thema/einen anderen Aspekt zu wählen und keine der
>    vorhandenen Fragen zu wiederholen (mit Fallback-Hinweis, falls noch keine existieren).
>    Zusätzlich `generationConfig = { temperature = 1.0 }` im `requestBody` ergänzen, um die
>    Varianz zu erhöhen. JSON-Antwortformat und Parse-Logik bleiben unverändert.

---

## Prompt 12 – Fehlende Create-View für manuelle MC-Fragen

> Fix the manual MC question creation error.
>
> Context: ASP.NET Core MVC project, branch `feature/azure-storage-config`.
> Beim Klick auf **"+ Frage manuell"** crasht Azure, weil die View
> `Views/MCQuestion/Create.cshtml` fehlt. Im Controller existieren bereits
> `GET Create(int chapterId)` und `POST Create(QuestionEditViewModel vm)`;
> `Edit.cshtml` und `Index.cshtml` sind vorhanden.
>
> Erstelle die fehlende View `Web_Engineering_Gruppenprojekt/Views/MCQuestion/Create.cshtml`.
> Keine Datenbankmodelle, `Program.cs` oder unrelated files ändern. Die View soll Stil und
> Struktur von `Views/MCQuestion/Edit.cshtml` wiederverwenden, `@model QuestionEditViewModel`
> nutzen, `ViewBag.Chapter` konsistent mit Index/Edit verwenden, per Formular an die
> POST-Create-Action senden, `@Html.AntiForgeryToken()` und ein hidden `ChapterId` enthalten.
> Felder für `QuestionText`, `Option1`, `Option2`, `Option3`, `Option4` und `CorrectOption`
> bereitstellen; `CorrectOption` soll Option 1 bis 4 auswählen lassen. Validation Summary
> und Validation Messages sollen konsistent mit `Edit.cshtml` sein. Submit-Button:
> **"Frage erstellen"**. Back-Link zu `MCQuestion/Index` mit `chapterId = Model.ChapterId`.
> Danach `dotnet build` ausführen.

Ergebnis: Die fehlende Razor-View wurde angelegt; betroffen war nur
`Web_Engineering_Gruppenprojekt/Views/MCQuestion/Create.cshtml`.

---

## Prompt 13 – Blob-Storage-Connection-String über ConnectionStrings lesen

> Ändere genau zwei Stellen, sonst nichts:
>
> 1. In Services/AzureBlobStorageService.cs: Ersetze
>    config["AzureBlobStorage:ConnectionString"]!
>    durch
>    config.GetConnectionString("AzureBlobStorage")!
>
> 2. In Program.cs: Ersetze
>    builder.Configuration["AzureBlobStorage:ConnectionString"]
>    durch
>    builder.Configuration.GetConnectionString("AzureBlobStorage")
>
> Die Zeile mit AzureBlobStorage:ContainerName bleibt unverändert. Ändere keine weiteren Dateien.

Ergebnis: Der Blob-Storage-Connection-String wird nun über `GetConnectionString("AzureBlobStorage")` aus dem ConnectionStrings-Bereich gelesen statt als App-Setting `AzureBlobStorage:ConnectionString`. Hintergrund: Die Azure-Subscription der Universität blockt per Policy App-Einstellungen mit dem Präfix `AzureBlobStorage`; der ConnectionStrings-Bereich (Typ Custom) ist davon nicht betroffen.

---

## Prompt 14 – Kapitel anlegen/bearbeiten: Course-Navigation aus ModelState entfernen

> In Controllers/ChapterController.cs sollen genau zwei Zeilen ergänzt werden, sonst nichts:
>
> 1. In der Methode [HttpPost] Create(Chapter chapter, IFormFile? slideFile): Füge als allererste Zeile im Methodenkörper ein:
>    ModelState.Remove(nameof(Chapter.Course));
>
> 2. In der Methode [HttpPost] Edit(int id, Chapter chapter, IFormFile? slideFile, bool deleteFile = false): Füge als allererste Zeile im Methodenkörper ein:
>    ModelState.Remove(nameof(Chapter.Course));
>
> Ändere keine anderen Methoden, keine anderen Dateien, keine Logik. Nur diese zwei Zeilen einfügen.

Ergebnis: In `Create` und `Edit` des `ChapterController` wird die non-nullable Navigation-Property `Course` vor der `ModelState.IsValid`-Prüfung aus dem ModelState entfernt. Zuvor schlug die Validierung fehl (das Formular übergibt nur `CourseId`, nicht das `Course`-Objekt), wodurch das Kapitel nie gespeichert wurde – ohne sichtbare Fehlermeldung.

---

## Prompt 15 – Gemini-Modell auf gemini-2.5-flash aktualisieren

> In Services/GeminiService.cs, Zeile 11: Ersetze in der const string BaseUrl genau den Modellnamen
> gemini-1.5-flash
> durch
> gemini-2.5-flash
>
> Die URL bleibt sonst exakt gleich (v1beta, generateContent unverändert). Ändere nichts anderes, keine weiteren Dateien.

Ergebnis: Der Modellname in der Gemini-BaseUrl wurde von `gemini-1.5-flash` auf `gemini-2.5-flash` geändert. `gemini-1.5-flash` liefert am `generateContent`-Endpunkt der v1beta-API einen HTTP 404 (von Google abgekündigt); mit `gemini-2.5-flash` funktioniert die KI-Fragengenerierung wieder.

---

## Prompt 16 – Alert-Verhalten nach KI-Fragengenerierung

> Prompt-Ziel: Verbessere das Alert-Verhalten auf der `MCQuestion`-Indexseite. Bestehende
> Bootstrap-Erfolgsalerts sollen nach etwa fünf Sekunden automatisch verschwinden. Beim
> erneuten Absenden von **"+ KI-Frage generieren"** sollen alte Erfolgs- oder Fehlermeldungen
> sofort ausgeblendet werden, während das bestehende Ladeverhalten mit Spinner und
> **"Wird generiert..."** erhalten bleibt.
>
> Keine Datenbankmodelle, Controller, Azure-Einstellungen oder unrelated files ändern.

Ergebnis: Die Alert-Logik wurde in `Views/MCQuestion/Index.cshtml` ergänzt: Success-Alerts
blenden automatisch aus, und alte Alerts werden beim Start einer neuen KI-Generierung
sofort verborgen.

---

## Prompt 17 – Verständliche Gemini-Rate-Limit-Meldung

> Prompt-Ziel: Verbessere die Fehlerbehandlung für Gemini-API-Rate-Limits bei
> KI-Fragengenerierung und KI-Prüfung. Gemini-Fehler wie HTTP 429,
> `RESOURCE_EXHAUSTED`, `quota exceeded`, `rate limit` oder ähnliche API-Fehlertexte
> sollen mit einer klaren deutschen Meldung angezeigt werden:
> **"Limit der Gemini API erreicht. Tageslimits werden voraussichtlich gegen 09:15 Uhr
> deutscher Zeit zurückgesetzt. Bitte später erneut versuchen."**
>
> Andere Fehler sollen wie bisher generisch behandelt werden. Keine Datenbankmodelle,
> Migrationen, Azure-Einstellungen oder unrelated files ändern.

Ergebnis: In `Services/GeminiService.cs` wurde eine Rate-Limit-Erkennung für Gemini-Fehler
ergänzt. `Controllers/MCQuestionController.cs` zeigt die Meldung bei KI-Fragengenerierung
als `TempData["Error"]` an; bei KI-Prüfung wird sie direkt im Modal-Feedback ausgegeben.

---

## Prompt 18 – Niedrige Gemini-Temperatur für deterministische KI-Ausgaben

> Prompt-Ziel: Setze eine niedrige Gemini-Temperatur für stabilere MC-Fragengenerierung
> und deterministischere KI-Prüfungen. Für KI-Fragengenerierung soll eine Temperatur um
> 0.3 verwendet werden; für KI-Prüfung eine Temperatur von 0.0 oder 0.1. Das bestehende
> Modell `gemini-2.5-flash`, Prompts, JSON-Parsing und die Rate-Limit-Behandlung sollen
> unverändert bleiben.
>
> Keine Datenbankmodelle, Migrationen, Azure-Einstellungen oder unrelated files ändern.

Ergebnis: In `Services/GeminiService.cs` wurde `generationConfig.temperature` für
KI-Fragengenerierung auf `0.3` und für KI-Prüfung auf `0.0` gesetzt.

---

## Prompt 19 – Lehrveranstaltung zuverlässig mit abhängigen Daten löschen

> Fix only the backend delete logic for deleting a Lehrveranstaltung.
>
> Context: Die Delete view für Lehrveranstaltungen existiert bereits und ist ausreichend.
> Sie zeigt eine Warnung mit der Anzahl der Kapitel, Fragen und Prüfungen und hat
> **"Löschen"**- und **"Abbrechen"**-Buttons.
>
> Problem: Beim Bestätigen des Löschens wirft die App HTTP 500. Azure-Logs zeigen eine
> `DbUpdateConcurrencyException` in `CourseController.DeleteConfirmed`; SQL-Logs deuten
> darauf hin, dass Course deletion und ExamQuestions deletion in einem `SaveChanges`-Batch
> gemischt werden.
>
> `CourseController.DeleteConfirmed` soll so korrigiert werden, dass eine Lehrveranstaltung
> mit Kapiteln, Fragen, Antwortoptionen und Prüfungen zuverlässig gelöscht wird. Keine
> zusätzliche Bestätigungsmodal oder Bestätigungsseite ergänzen, die bestehende Delete-View
> weitgehend unverändert lassen, keine Datenbankmodelle, Migrationen, Azure-Einstellungen
> oder unrelated files ändern. Abhängige Daten sollen explizit und in sicherer Reihenfolge
> per `ExecuteDeleteAsync` gelöscht werden: `ExamQuestions` der Prüfungen des Kurses,
> `ExamQuestions` der Fragen aus Kurskapiteln, `AnswerOptions`, `Questions`, `Exams`,
> `Chapters`, danach der `Course` selbst; die Operation soll in einer Transaktion laufen.
> Falls der Kurs nicht mehr existiert, sicher zu `Index` weiterleiten.

Ergebnis: Die Löschlogik wurde in
`Web_Engineering_Gruppenprojekt/Controllers/CourseController.cs` auf explizite
`ExecuteDeleteAsync`-Löschungen in sicherer Reihenfolge umgestellt. Die bestehende
Delete-View blieb erhalten; es wurde keine zusätzliche Bestätigungsmodal und keine
zusätzliche Bestätigungsseite ergänzt.

---

## Prompt 20 – Azure-SQL-Retry-Strategie für Delete-Transaktion

> Fix the `CourseController.DeleteConfirmed` transaction for Azure SQL retry execution
> strategy.
>
> Context: Der vorherige Backend-Delete-Fix schlägt auf Azure SQL mit dem Runtime-Fehler
> fehl, dass `SqlServerRetryingExecutionStrategy` keine user-initiierten Transaktionen
> unterstützt. Die Transaktion soll über die von
> `DbContext.Database.CreateExecutionStrategy()` zurückgegebene Execution Strategy als
> retriable unit ausgeführt werden.
>
> Aktualisiere nur `CourseController.DeleteConfirmed`: Die bestehende Delete-View
> unverändert lassen, die explizite sichere Löschreihenfolge und die Transaktion
> beibehalten, aber die gesamte transaktionale Löschoperation in
> `var strategy = db.Database.CreateExecutionStrategy(); await strategy.ExecuteAsync(async () => { ... });`
> kapseln. Innerhalb des Execution-Strategy-Blocks die Transaktion öffnen, alle
> `ExecuteDeleteAsync`-Operationen in sicherer Reihenfolge ausführen und die Transaktion
> committen. Keine Transaktion außerhalb des Blocks starten, kein `SaveChangesAsync` für
> bereits per `ExecuteDeleteAsync` gelöschte Entitäten verwenden, bei nicht vorhandenem Kurs
> sicher zu `Index` weiterleiten. Keine Datenbankmodelle, Migrationen, Azure-Einstellungen
> oder unrelated files ändern.

Ergebnis: In `Web_Engineering_Gruppenprojekt/Controllers/CourseController.cs` wird die
Delete-Transaktion nun innerhalb von `CreateExecutionStrategy().ExecuteAsync(...)`
geöffnet und committed. Die bestehende Delete-View blieb erhalten; es wurde keine
zusätzliche Bestätigungsmodal und keine zusätzliche Bestätigungsseite ergänzt.

---

## Beispiel für einen Zusammenfassungs-Prompt (Meta-Technik)

> Fasse die letzten Schritte, mit denen wir die Fragen- und Prüfungsverwaltung implementiert
> haben, in einem einzigen, strukturierten Prompt zusammen, der ausreicht, um das Feature
> von Grund auf erneut zu erzeugen – mit klarer Auflistung von Datenmodell-Bezug,
> Controller-Aktionen, Views und Bootstrap-Komponenten.

Diese Technik wurde genutzt, um aus vielen kleinen iterativen Anweisungen die oben stehenden
verdichteten Feature-Prompts zu erzeugen.

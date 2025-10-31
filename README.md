# Deck of Cards - Högre eller Lägre Spel

## 📖 Vad är detta projekt?

Detta är ett webbaserat kortspel där spelaren gissar om nästa kort kommer att ha ett högre eller lägre värde än det nuvarande kortet. Projektet är byggt med moderna .NET-tekniker och består av tre huvuddelar:

1. **Backend API** (`deck-of-cards-api`) - Hanterar kortlogik och kommunicerar med externa API:er
2. **Frontend Webbapplikation** (`DeckOfCardsWeb`) - Interaktivt användargränssnitt byggt med Blazor
3. **Testprojekt** (`deck-of-cards-test`) - Enhetstester för att säkerställa kvalitet

### Vad gör applikationen?

- 🎮 **Kortspel**: Spelaren drar kort och gissar om nästa kort är högre eller lägre
- 📊 **Poängräkning**: Systemet räknar automatiskt poäng för rätt gissningar
- 🔄 **Status & Loggar**: En dedikerad sida för att övervaka systemets hälsa och loggar
- 🎯 **Health Checks**: Backend API exponerar health check endpoints för övervakning
- 📝 **Loggning**: Omfattande loggning med Serilog för debugging och övervakning
- 🎨 **Responsiv Design**: Fungerar på både desktop och mobil enheter

## 🎮 Hur fungerar spelet?

### Spelregler
1. **Starta spelet**: Klicka på "Draw Card" för att få ditt första kort
2. **Gissa värdet**: Välj "Higher" (högre) eller "Lower" (lägre) baserat på om du tror nästa kort har ett högre eller lägre värde än det nuvarande
3. **Poäng**: Varje rätt gissning ger dig 1 poäng
4. **Historik**: Alla tidigare kort visas så du kan se din progress
5. **Game Over**: Vid fel gissning avslutas spelet och du kan starta ett nytt

### Kortvärden
- **Ess (Ace)**: 1
- **Nummer 2-10**: Nominellt värde (2, 3, 4, ... 10)
- **Knekt (Jack)**: 11
- **Dam (Queen)**: 12
- **Kung (King)**: 13

## 🛠️ Förutsättningar

Innan du börjar, se till att du har följande installerat:

### Obligatoriska verktyg
- **.NET 9.0 SDK** eller senare
  - Ladda ner från [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
  - Verifiera installationen: `dotnet --version` (ska visa 9.0.x eller senare)

### Rekommenderade verktyg (välj en)
- **Visual Studio 2022** (Community Edition är gratis)
  - Inkluderar alla verktyg du behöver
  - Ladda ner från [visualstudio.microsoft.com](https://visualstudio.microsoft.com/)
- **Visual Studio Code** + C# Extension
  - Lättviktig editör med bra .NET-stöd
  - Ladda ner från [code.visualstudio.com](https://code.visualstudio.com/)
- **Rider** (JetBrains)
  - Professionell IDE (kostar men har gratis testperiod)

### Ytterligare krav
- **Git** - För versionskontroll
- **Webbläsare** - Chrome, Firefox, Edge eller Safari (senaste versionen)
- **Internetanslutning** - Applikationen använder extern API för kort

## 🚀 Snabbstart - Första gången

### Steg 1: Klona projektet

Öppna terminalen (Command Prompt, PowerShell eller Terminal) och kör:

```bash
# Klona projektet från Git
git clone [repository-url]

# Navigera till projektmappen
cd deck-of-cards-higher-lower
```

### Steg 2: Återställ NuGet-paket

.NET behöver ladda ner alla paket som projektet använder:

```bash
# Återställ paket för alla projekt i solution-filen
dotnet restore
```

Detta kan ta några minuter första gången.

### Steg 3: Bygg projektet

Kontrollera att allt kompilerar korrekt:

```bash
# Bygg alla projekt
dotnet build
```

Du bör se meddelanden som "Build succeeded". Om du ser fel, se felsökningsavsnittet nedan.

### Steg 4: Starta Backend API

Öppna en terminal och navigera till API-projektet:

```bash
cd deck-of-cards-api
dotnet run
```

Du bör se meddelanden som:
```
info: Program[0]
      Starting Deck of Cards API - Observability: enabled
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5040
```

**Viktigt**: Låt denna terminal vara öppen och körande!

### Steg 5: Starta Frontend (i ny terminal)

Öppna en **ny terminal** (behåll API:et körande) och kör:

```bash
cd DeckOfCardsWeb
dotnet run
```

Du bör se meddelanden som:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7214
```

### Steg 6: Öppna i webbläsaren

Öppna din webbläsare och gå till:
- **HTTPS**: `https://localhost:7214` (rekommenderas)
- **HTTP**: `http://localhost:5129`

Om du får ett säkerhetsvarning om certifikat, klicka "Avancera" och "Fortsätt ändå" (detta är normalt i utveckling).

### Steg 7: Testa applikationen

1. Klicka på "Draw Card" för att starta spelet
2. Välj "Higher" eller "Lower" för nästa kort
3. Se din poäng öka!
4. Besök "Status & Logs" i menyn för att se systemstatus

## 🧪 Testning av applikationen

### Köra enhetstester

Projektet inkluderar automatiska tester som verifierar att allt fungerar korrekt.

#### Kör alla tester

Från projektets rotmapp:

```bash
# Kör alla tester i testprojektet
dotnet test
```

#### Kör tester med detaljerad output

För mer information om vad som testas:

```bash
# Med verbose output
dotnet test --verbosity normal

# Med detaljerad output
dotnet test --verbosity detailed
```

#### Kör tester med coverage (täckning)

För att se hur mycket av koden som testas:

```bash
# Kör tester och generera coverage rapport
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Testa API:et manuellt

Du kan testa API:et direkt med olika verktyg:

#### Med curl (kommandorad)

```bash
# Testa health check
curl http://localhost:5040/health

# Testa att skapa en kortlek
curl http://localhost:5040/cards/deck/new

# Testa status endpoint
curl http://localhost:5040/status/health
curl http://localhost:5040/status/logs
```

#### Med HTTP-fil (REST Client)

Projektet innehåller en `cards-test.http` fil som kan användas med:
- **Visual Studio Code**: Installera "REST Client" extension
- **Rider**: Inbyggt stöd
- **Visual Studio**: Installera "Rest Client" extension

Öppna filen `deck-of-cards-api/cards-test.http` för att se exempel på API-anrop.

#### Med Postman eller Insomnia

1. Skapa en ny GET request
2. URL: `http://localhost:5040/health`
3. Klicka "Send"

### Testa Frontend manuellt

1. Starta både backend och frontend (se ovan)
2. Öppna webbläsaren och navigera till applikationen
3. Testa följande funktioner:
   - ✅ Starta ett nytt spel
   - ✅ Gissa högre/lägre
   - ✅ Se poängräkning
   - ✅ Testa game over-scenariot
   - ✅ Navigera till Status & Logs-sidan
   - ✅ Verifiera att loggar visas korrekt
   - ✅ Verifiera att health status visas

## 📁 Projektstruktur

För att förstå hur projektet är organiserat:

```
deck-of-cards-higher-lower/
├── deck-of-cards-api/          # Backend Web API
│   ├── Extensions/              # API endpoints (routing)
│   │   └── DeckOfCardsEndpoints.cs
│   ├── Models/                  # Datamodeller
│   │   └── DeckOfCards.cs
│   ├── Services/                # Affärslogik
│   │   ├── DeckOfCardsService.cs
│   │   ├── HealthCheckService.cs
│   │   └── FeatureToggleService.cs
│   ├── logs/                    # Applikationsloggar (skapas automatiskt)
│   ├── Program.cs               # Applikationens startpunkt
│   └── appsettings.json         # Konfiguration
│
├── DeckOfCardsWeb/             # Frontend Blazor-applikation
│   ├── Components/              # Blazor-komponenter
│   │   ├── Pages/               # Sidor (Home, Status, Error)
│   │   └── Layout/              # Layout-komponenter (meny, etc.)
│   ├── Models/                  # Datamodeller för frontend
│   │   └── CardModels.cs
│   ├── Services/                # Frontend-tjänster
│   │   ├── CardService.cs       # Kommunikation med API
│   │   ├── StatusService.cs     # Status och loggar
│   │   └── FeatureToggleService.cs
│   ├── wwwroot/                 # Statiska filer (CSS, JS, bilder)
│   ├── Program.cs               # Applikationens startpunkt
│   └── appsettings.json         # Konfiguration
│
├── deck-of-cards-test/          # Enhetstester
│   ├── DeckOfCardsApiTests.cs   # API-tester
│   └── DeckOfCardsServiceTests.cs # Service-tester
│
└── README.md                    # Denna fil
```

### Viktiga filer förstå

#### `Program.cs`
Startpunkten för applikationen. Här konfigureras:
- Dependency Injection
- Middleware
- Routing
- Services

#### `appsettings.json`
Konfigurationsfiler som innehåller:
- API-URLs
- Loggningsinställningar
- Feature flags
- Övriga inställningar

#### Blazor-komponenter (`.razor`-filer)
Blazor använder `.razor`-filer som kombinerar HTML och C#:
- `@page` - Definierar routning
- `@code` - C#-kod för logik
- HTML-markup - Användargränssnittet

## 🔧 Vanliga utvecklingsuppgifter

### Lägga till en ny sida

1. Skapa en ny `.razor`-fil i `DeckOfCardsWeb/Components/Pages/`:
   ```razor
   @page "/my-new-page"
   <h1>Min nya sida</h1>
   ```

2. Lägg till länk i navigationsmenyn (`NavMenu.razor`):
   ```razor
   <NavLink class="nav-link" href="/my-new-page">
       Min sida
   </NavLink>
   ```

### Lägga till en ny API-endpoint

1. Öppna `deck-of-cards-api/Extensions/DeckOfCardsEndpoints.cs`
2. Lägg till endpoint i `MapDeckOfCardsEndpoints`-metoden:
   ```csharp
   app.MapGet("/my-endpoint", MyEndpointHandler)
       .WithName("MyEndpoint")
       .WithOpenApi();
   ```
3. Implementera handler-metoden längre ner i filen

### Lägga till en ny tjänst

1. Skapa en ny klass i `Services/`-mappen
2. Registrera i `Program.cs`:
   ```csharp
   builder.Services.AddScoped<MyNewService>();
   ```
3. Använd dependency injection för att använda tjänsten:
   ```csharp
   public class MyComponent
   {
       [Inject]
       public MyNewService MyService { get; set; }
   }
   ```

### Ändra API-URL

Redigera `DeckOfCardsWeb/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5040"  // Ändra här
  }
}
```

### Aktivera/inaktivera feature flags

Redigera `appsettings.json` eller `appsettings.Development.json`:

```json
{
  "FeatureToggles": {
    "WelcomeGreeting": true,  // true = aktiverad, false = inaktiverad
    "Observability": true
  }
}
```

## 🐛 Felsökning

### Problem: "Port already in use"

**Felmeddelande**: `Failed to bind to address http://localhost:5040`

**Lösning**:
1. Hitta processen som använder porten:
   ```bash
   # Windows PowerShell
   netstat -ano | findstr :5040
   
   # Mac/Linux
   lsof -i :5040
   ```
2. Stoppa processen eller ändra port i `launchSettings.json`

### Problem: "Cannot connect to API"

**Felmeddelande**: `Failed to connect` eller timeout

**Lösning**:
1. Kontrollera att backend API:et är startat och körs
2. Verifiera att URL:en i `appsettings.json` matchar API:ets port
3. Testa API:et direkt i webbläsaren: `http://localhost:5040/health`
4. Kontrollera brandväggsinställningar

### Problem: "Certificate errors"

**Felmeddelande**: Säkerhetsvarningar om certifikat

**Lösning**:
1. Detta är normalt i utveckling
2. Klicka "Avancera" → "Fortsätt ändå" i webbläsaren
3. Alternativt: Använd HTTP istället för HTTPS

### Problem: Tester misslyckas

**Felmeddelande**: `Test run failed`

**Lösning**:
1. Se till att API:et inte körs när tester körs (de startar sitt eget)
2. Kontrollera att alla NuGet-paket är installerade: `dotnet restore`
3. Bygg projektet först: `dotnet build`
4. Kör tester med mer detaljer: `dotnet test --verbosity detailed`

### Problem: "NuGet packages missing"

**Felmeddelande**: `The type or namespace name 'X' could not be found`

**Lösning**:
```bash
# Återställ alla NuGet-paket
dotnet restore

# Om det inte fungerar, rensa och återställ
dotnet clean
dotnet restore
dotnet build
```

### Problem: Loggar syns inte

**Felmeddelande**: Inga loggar på Status-sidan

**Lösning**:
1. Kontrollera att `FeatureToggles:Observability` är `true` i `appsettings.json`
2. Verifiera att `logs/`-mappen finns och innehåller filer
3. Kontrollera att API:et har skrivrättigheter till `logs/`-mappen

## 📚 Ytterligare resurser

### Officiell dokumentation
- **ASP.NET Core**: [docs.microsoft.com/aspnet/core](https://docs.microsoft.com/aspnet/core)
- **Blazor**: [blazor.net](https://blazor.net)
- **.NET**: [dotnet.microsoft.com/learn](https://dotnet.microsoft.com/learn)

### Lär dig mer
- **Blazor Tutorial**: [Microsoft Learn - Blazor](https://learn.microsoft.com/aspnet/core/blazor/)
- **REST API Best Practices**: [REST API Tutorial](https://restfulapi.net/)
- **C# Grundkurs**: [Microsoft Learn - C#](https://learn.microsoft.com/dotnet/csharp/)

### Hjälp och support
- **Stack Overflow**: Tagga med `asp.net-core`, `blazor`, `csharp`
- **GitHub Issues**: Skapa ett issue i projektets repository för buggar

## 🎯 Nästa steg för utveckling

När du är bekväm med grunderna, kan du:

1. **Lägga till fler features**
   - High score-lista
   - Flerspelarläge
   - Svårighetsgrader

2. **Förbättra användarupplevelsen**
   - Animationer
   - Ljudeffekter
   - Tema-stöd

3. **Förbättra kvaliteten**
   - Lägg till fler tester
   - Implementera CI/CD
   - Code reviews

4. **Optimerisering**
   - Caching
   - Prestanda-förbättringar
   - Säkerhetsförbättringar

## 📝 API Endpoints Referens

### Kortspel Endpoints

| Metod | Endpoint | Beskrivning |
|-------|----------|-------------|
| GET | `/cards/deck/new?deckCount=1` | Skapar en ny kortlek |
| GET | `/cards/deck/{deckId}/draw?count=1` | Drar kort från en kortlek |
| GET | `/cards/deck/{deckId}/draw-five` | Drar exakt 5 kort |
| GET | `/cards/deck/{deckId}/shuffle` | Blandar kortleken |

### Status Endpoints

| Metod | Endpoint | Beskrivning |
|-------|----------|-------------|
| GET | `/health` | Basic health check |
| GET | `/health/ready` | Readiness check (inkluderar 'ready' taggade checks) |
| GET | `/health/live` | Liveness check |
| GET | `/status/health` | Detaljerad health status med alla checks |
| GET | `/status/logs?limit=100` | Hämta applikationsloggar |

### Exempel på API-anrop

```bash
# Skapa en ny kortlek
curl http://localhost:5040/cards/deck/new

# Dra 3 kort från en kortlek (ersätt {deckId} med faktiskt ID)
curl http://localhost:5040/cards/deck/abc123/draw?count=3

# Hämta health status
curl http://localhost:5040/status/health

# Hämta senaste 50 loggar
curl http://localhost:5040/status/logs?limit=50
```

## 🤝 Bidrag till projektet

Vi välkomnar alla bidrag! Så här gör du:

1. **Forka** projektet
2. **Skapa** en feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** dina ändringar (`git commit -m 'Add some AmazingFeature'`)
4. **Push** till branchen (`git push origin feature/AmazingFeature`)
5. **Öppna** en Pull Request

### Kodexempel och stil
- Följ C# coding conventions
- Skriv tester för ny funktionalitet
- Uppdatera dokumentation när det behövs
- Kommentera komplicerad logik

## 📄 Licens

Detta projekt är skapat för utbildningsändamål.

## 👥 Författare och erkännanden

Projektet använder:
- [Deck of Cards API](https://deckofcardsapi.com/) - Externa kort-API:et
- [Bootstrap](https://getbootstrap.com/) - CSS-ramverk
- [Serilog](https://serilog.net/) - Strukturerad loggning

---

**Glöm inte**: Om du fastnar, ta det lugnt! Utveckling handlar om att lära sig, och alla börjar någonstans. Använd dokumentationen, Google och fråga om hjälp när du behöver det. Lycka till! 🚀

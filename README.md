# Deck of Cards - Högre eller Lägre Spel

## Beskrivning
Detta är ett webbaserat kortspel byggt med ASP.NET Core Blazor Server där spelaren gissar om nästa kort kommer att ha ett högre eller lägre värde än det nuvarande kortet.

## Projektstruktur
Projektet består av två huvuddelar:

### DeckOfCardsWeb
En Blazor Server-applikation som innehåller:
- **Frontend**: Interaktivt användargränssnitt för kortspelet
- **Backend**: Tjänster för att hantera kortlogik och API-anrop

### test-api
En ASP.NET Core Web API som tillhandahåller:
- **REST API**: Endpoints för korthantering
- **Kortlogik**: Tjänster för att skapa kortlekar och dra kort

## Spelregler
1. **Starta spelet**: Klicka på "Draw Card" för att få ditt första kort
2. **Gissa**: Välj "Higher" eller "Lower" baserat på om du tror nästa kort har ett högre eller lägre värde
3. **Poäng**: Varje rätt gissning ger 1 poäng
4. **Historik**: Alla tidigare kort visas så du kan se din progress
5. **Game Over**: Vid fel gissning avslutas spelet och du kan starta ett nytt

## Kortvärden
- **Ess**: 1
- **Nummer 2-10**: Nominellt värde
- **Knekt**: 11
- **Dam**: 12
- **Kung**: 13

## Teknikstack
- **Frontend**: Blazor Server (ASP.NET Core 9.0)
- **Backend**: ASP.NET Core Web API
- **Styling**: Bootstrap CSS
- **Kort API**: Deck of Cards API (deckofcardsapi.com)

## Funktioner
- 🎮 Interaktivt kortspel med realtidsuppdateringar
- 📊 Poängsystem med löpande räkning
- 🎨 Responsiv design med Bootstrap
- 🔄 Automatisk korthantering från extern API
- 📱 Fungerar på både desktop och mobil
- 🎯 Visual feedback för spelresultat

## Installation och Körning

### Förutsättningar
- .NET 9.0 SDK
- Visual Studio 2022 eller VS Code

### Steg för steg
1. **Klona projektet**
   ```bash
   git clone [repository-url]
   cd if-tester
   ```

2. **Bygg projektet**
   ```bash
   dotnet build
   ```

3. **Starta API:et** (i en terminal)
   ```bash
   cd test-api
   dotnet run
   ```

4. **Starta webbapplikationen** (i en annan terminal)
   ```bash
   cd DeckOfCardsWeb
   dotnet run
   ```

5. **Öppna webbläsaren**
   - Navigera till `https://localhost:5001` (eller den port som visas i terminalen)

## Utveckling
Projektet använder:
- **Blazor Server**: För interaktiv frontend med server-side rendering
- **Dependency Injection**: För tjänstehantering
- **HTTP Client**: För externa API-anrop
- **Bootstrap**: För responsiv UI-design

## API Endpoints
### test-api
- `GET /api/deck/new` - Skapar en ny kortlek
- `GET /api/deck/{deckId}/draw/{count}` - Drar kort från kortlek

## Framtida Förbättringar
- [ ] Multiplayer-funktionalitet
- [ ] Lokalt sparande av high scores
- [ ] Olika svårighetsgrader
- [ ] Animationer för kortdragning
- [ ] Ljud- och musikeffekter

## Bidrag
Projektet välkomnar bidrag! Skapa en issue eller pull request för förbättringar.

## Licens
Detta projekt är skapat för utbildningsändamål.
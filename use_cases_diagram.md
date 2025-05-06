# Use Case Diagrams for Ordination System

## Use Case Overview

```mermaid
graph TD
    Actor((Læge)) --> UC1[UC1: Opret Patient Ordination]
    Actor --> UC2[UC2: Anvend PN Ordination]
    Actor --> UC3[UC3: Vis Statistik]
    
    UC1 --> UC1_1[Opret PN Ordination]
    UC1 --> UC1_2[Opret DagligFast Ordination]
    UC1 --> UC1_3[Opret DagligSkæv Ordination]
    
    UC3 --> UC3_1[Vis ordinationer per lægemiddel]
    UC3 --> UC3_2[Vis patient ordinationsdetaljer]
```

## UC1: Opret Patient Ordination - Sequence Diagram

```mermaid
sequenceDiagram
    actor Læge
    participant UI as Brugergrænsefladen
    participant Service as DataService
    participant DB as Database
    
    Læge->>UI: Vælg patient
    Læge->>UI: Vælg lægemiddel
    Læge->>UI: Vælg ordinationstype
    
    alt PN Ordination
        Læge->>UI: Angiv PN dosis
        Læge->>UI: Angiv start- og slutdato
        UI->>Service: opretPN(patient, lægemiddel, dosis, startDato, slutDato)
        Service->>DB: Gem PN ordination
        DB-->>Service: Bekræft
        Service-->>UI: Bekræft
    else DagligFast Ordination
        Læge->>UI: Angiv morgen-, middag-, aften- og nat-dosis
        Læge->>UI: Angiv start- og slutdato
        UI->>Service: opretDagligFast(patient, lægemiddel, dosisMorgen, dosisMiddag, dosisAften, dosisNat, startDato, slutDato)
        Service->>DB: Gem DagligFast ordination
        DB-->>Service: Bekræft
        Service-->>UI: Bekræft
    else DagligSkæv Ordination
        Læge->>UI: Angiv doser med tidspunkter
        Læge->>UI: Angiv start- og slutdato
        UI->>Service: opretDagligSkaev(patient, lægemiddel, doser, startDato, slutDato)
        Service->>DB: Gem DagligSkæv ordination
        DB-->>Service: Bekræft
        Service-->>UI: Bekræft
    end
    
    UI-->>Læge: Vis bekræftelse
```

## UC2: Anvend PN Ordination - Sequence Diagram

```mermaid
sequenceDiagram
    actor Læge
    participant UI as Brugergrænsefladen
    participant Service as DataService
    participant PN as PN Ordination
    participant DB as Database
    
    Læge->>UI: Vælg patient
    UI->>Service: GetPatienter()
    Service-->>UI: Liste af patienter
    
    Læge->>UI: Vælg PN ordination
    UI->>Service: GetOrdinationerForPatient(patientId)
    Service-->>UI: Liste af ordinationer
    
    Læge->>UI: Angiv dato for anvendelse
    UI->>Service: anvendOrdinationPN(ordination, dato)
    Service->>PN: givDosis(dato)
    
    alt Dato er gyldig (inden for ordinationsperioden)
        PN->>DB: Gem anvendt dosis
        DB-->>PN: Bekræft
        PN-->>Service: True
        Service-->>UI: True
        UI-->>Læge: Vis bekræftelse
    else Dato er ugyldig
        PN-->>Service: False
        Service-->>UI: False
        UI-->>Læge: Vis fejlbesked
    end
```

## UC3: Vis Statistik - Sequence Diagram

```mermaid
sequenceDiagram
    actor Læge
    participant UI as Brugergrænsefladen
    participant Service as DataService
    participant DB as Database
    
    alt Vis ordinationer per lægemiddel
        Læge->>UI: Vælg "Vis statistik"
        UI->>Service: GetOrdinationerPerLægemiddel()
        Service->>DB: Hent data
        DB-->>Service: Data
        Service-->>UI: Statistik data
        UI-->>Læge: Vis statistik
    else Vis patient ordinationsdetaljer
        Læge->>UI: Vælg patient
        UI->>Service: GetPatienter()
        Service-->>UI: Liste af patienter
        
        Læge->>UI: Vælg "Vis ordinationer"
        UI->>Service: GetOrdinationerForPatient(patientId)
        Service->>DB: Hent ordinationer
        DB-->>Service: Ordinationer
        Service-->>UI: Ordinationsdata
        UI-->>Læge: Vis ordinationsdetaljer
    end
```

## Class Relationship Diagram

```mermaid
classDiagram
    class Patient {
        +string cprnr
        +string navn
        +double vægt
        +List~Ordination~ ordinationer
        +setVægt(double) bool
    }
    
    class Ordination {
        <<abstract>>
        +Dato startDato
        +Dato slutDato
        +Laegemiddel laegemiddel
        +double samletDosis()
        +double doegnDosis()
        +Dato getStartDato()
        +Dato getSlutDato()
    }
    
    class PN {
        +double antalEnheder
        +List~Dato~ dates
        +bool givDosis(Dato)
        +double doegnDosis()
        +double samletDosis()
    }
    
    class DagligFast {
        +Dosis morgen
        +Dosis middag
        +Dosis aften
        +Dosis nat
        +double doegnDosis()
        +double samletDosis()
    }
    
    class DagligSkæv {
        +List~Dosis~ doser
        +double doegnDosis()
        +double samletDosis()
    }
    
    class Laegemiddel {
        +string navn
        +double enhedPrKgPrDoegnLet
        +double enhedPrKgPrDoegnNormal
        +double enhedPrKgPrDoegnTung
        +double getEnhedPrKgPrDoegn(Patient)
    }
    
    class Dosis {
        +Dato tid
        +double antal
        +Dato getTid()
        +double getAntal()
    }
    
    class Dato {
        +int dag
        +int måned
        +int år
        +bool erFørEl(Dato)
    }
    
    Patient "1" --> "*" Ordination
    Ordination <|-- PN
    Ordination <|-- DagligFast
    Ordination <|-- DagligSkæv
    Ordination "*" --> "1" Laegemiddel
    DagligFast "*" --> "4" Dosis
    DagligSkæv "*" --> "*" Dosis
    PN "*" --> "*" Dato
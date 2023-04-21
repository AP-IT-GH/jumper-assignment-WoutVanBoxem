# jumper-assignment-WoutVanBoxem
jumper-assignment-WoutVanBoxem created by GitHub Classroom
# CubeAgent ML-Agent Documentatie

Dit is de documentatie voor de `CubeAgent` Unity ML-Agent. Deze agent is ontworpen om te leren springen om een blok te raken of te vermijden, afhankelijk van het doel voor het huidige episode.

## Inhoud

1. [Belangrijkste componenten](#belangrijkste-componenten)
2. [Start functie](#start-functie)
3. [OnEpisodeBegin](#onepisodebegin)
4. [CollectObservations](#collectobservations)
5. [OnActionReceived](#onactionreceived)
6. [OnCollisionEnter](#oncollisionenter)
7. [FixedUpdate](#fixedupdate)
8. [Training](#training)

## Belangrijkste componenten

De volgende componenten zijn essentieel voor het begrijpen van de CubeAgent:

- `cuboid`: Een referentie naar het GameObject dat de agent moet raken of vermijden.
- `cuboidRb`: Een verwijzing naar de Rigidbody van het `cuboid` GameObject.
- `shouldHitCuboid`: Een booleaanse waarde die aangeeft of de agent het `cuboid` in het huidige episode moet raken of vermijden.
- `maxCuboidXPosition`: De maximale x-positie van het `cuboid` voordat het episode eindigt.
- `jumpCooldown`: De tijd tussen sprongen (in seconden).

## Start functie

De `Start` functie initialiseert de agent en beperkt de bewegingsvrijheid van zowel de agent als het `cuboid`.

```csharp
public void Start(){
    //...
}
```
## OnEpisodeBegin

De `OnEpisodeBegin` methode wordt aangeroepen bij het begin van elk episode en initialiseert de agent en het `cuboid` GameObject.

```csharp
public override void OnEpisodeBegin(){
    //...
}
```
## CollectObservations

De `CollectObservations` methode verzamelt observaties die aan de ML-Agent worden doorgegeven om te leren.

```csharp
public override void CollectObservations(VectorSensor sensor){
    // Observe cube position
    sensor.AddObservation(transform.position);

    // Observe cuboid position
    sensor.AddObservation(cuboid.transform.position);

    // Observe cuboid velocity
    sensor.AddObservation(cuboidRb.velocity.x);

    // Observe whether should hit cuboid
    sensor.AddObservation(shouldHitCuboid);
}
```
## OnActionReceived

De `OnActionReceived` methode wordt aangeroepen wanneer de agent een actie moet uitvoeren. In dit geval is de enige actie die de agent kan uitvoeren springen.

```csharp
public override void OnActionReceived(ActionBuffers actions){
    // Jump action
    if (actions.DiscreteActions[0] == 1 && transform.position.y <= 1f)
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 39.24f);
        timeSinceLastJump = 0f;
        // Reward for jumping when should not hit cuboid
        if (!shouldHitCuboid)
        {
            AddReward(0.5f);
        }
        // Penalty for jumping when should hit cuboid
        else if (shouldHitCuboid)
        {
            AddReward(-1.0f);
        }
    }
}
```
## OnCollisionEnter

De `OnCollisionEnter` methode wordt aangeroepen wanneer de agent botst met een ander GameObject. In dit geval wordt deze methode gebruikt om te bepalen of de agent het `cuboid` heeft geraakt en de bijbehorende beloning of straf toe te passen.

```csharp
private void OnCollisionEnter(Collision collision){
    // Reward for hitting cuboid when should hit cuboid
    if (shouldHitCuboid && collision.gameObject.CompareTag("Cuboid"))
    {
        AddReward(1.0f);
        EndEpisode();
    }
    // Penalty for hitting cuboid when should not hit cuboid
    else if (!shouldHitCuboid && collision.gameObject.CompareTag("Cuboid"))
    {
        AddReward(-1.0f);
        EndEpisode();
    }
}
```
## FixedUpdate

De `FixedUpdate` methode wordt aangeroepen tijdens elke vaste update van de fysica en wordt gebruikt om het gedrag van de agent te controleren en de bijbehorende beloningen of straffen toe te passen.

```csharp
private void FixedUpdate(){
    // Check if cuboid is below a certain height and the CubeAgent is above it
    if (!shouldHitCuboid && cuboid.transform.position.y < 1f && transform.position.y > 1f)
    {
        AddReward(1.0f);
        EndEpisode();
    }
    // Penalty for not jumping when should not hit cuboid
    else if (shouldHitCuboid && cuboid.transform.position.x < transform.position.x && transform.position.y <= 0.5f)
    {
        AddReward(-1.0f);
        EndEpisode();
    }
    if (cuboid.transform.position.x > maxCuboidXPosition)
    {
        EndEpisode();
    }
    timeSinceLastJump += Time.fixedDeltaTime;
}
```

## Training

De training van de ML-agent verliep succesvol, waarbij de standaardafwijking van de beloningen van elke afzonderlijke episode consistent afnam, terwijl de gemiddelde beloning per episode geleidelijk toenam. Dit duidt op een geleidelijke verbetering van het gedrag van de agent tijdens het leerproces en een toename van de nauwkeurigheid en effectiviteit van de besluitvorming en acties van de agent. De geleidelijke afname van de standaardafwijking van de beloningen toont aan dat de agent consistent presteert en dat de algoritmen en parameters van het model goed zijn afgestemd op de gegeven taak. Deze resultaten geven aan dat de training van de ML-agent succesvol is en dat het getrainde model in staat is om nauwkeurig te voorspellen en acties uit te voeren voor de gegeven taak.
![image](https://user-images.githubusercontent.com/91462836/233742031-e1588cf4-4970-4884-8cd5-1288388c2e3c.png)


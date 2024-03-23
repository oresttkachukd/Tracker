## Communication

As the services will be running within different networks without direct access to each other, an Event hub (Kafka) was chosen as the communication layer. This approach fully decouples the services, ensuring that the Pixel service can remain highly available and responsive without impacting response times. The Storage service can then process events at its own pace without needing to meet high availability requirements
The solution doesn't employ any authorization to Kafka as it assumes it's running in a secure network, e.g. private shared VNet. Dead letter and retry queues were not introduced. Consumer implementation is basic and doesn't support batching or concurrent processing.

## Building blocks

Genereally not recommended practice to reuse code between microservices, but for such widely used dependencies as Event hub it was decided to introduce shared Building Block folder.

## Pixel.Service

The Pixel service is kept simple by defining the entire configuration and processing pipeline in Program.cs without splitting it into separate classes. 

Further clarification is required on the following aspects:
- To reduce additional server IO, costs, and traffic, we could utilize public CDN services. The decision should be based on specific requirements as this introduces an additional initial round trip, but it would enable us to both process every /track request without caching and cache locally on customers machines image responses.
The implementation would be simple, replacing FileResult with a CDN url pointing to the file. 
- Currently, caching for the endpoint is disabled to ensure that every subsequent request is processed and stored. In-memory caching is utilized to load the image only once.

CORS were enabled to be able to access to website from other websites.

The IP address is a mandatory value, but potential empty values are still sent to the Event hub topic for analysis purposes.

To improve availability, service won't wait for Event hub aknowledge of the message. Messages will be batched locally on the server and sent as batch.

## Storage.Worker

Given that piblic access is not required for the storage service, a Worker type of instance was chosen. It utilizes a smaller runtime Docker base image for improved efficiency.

## Instructions

To run solution following instructions can be used:

```bash
# Go into the folder with solution and run:
$ docker-compose up
```

To override any settings, environment variables prefixed with TRACKER__ can be added to a .env file. For example:

```bash
TRACKER__KAFKA_SERVERS=...
```
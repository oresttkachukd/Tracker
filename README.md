## Communication

As the services will be running within different networks without direct access to each other, an Event hub (Kafka) was chosen as the communication layer. This approach fully decouples the services, ensuring that the Pixel service can remain highly available and responsive without impacting response times. The Storage service can then process events at its own pace without needing to meet high availability requirements
The solution doesn't employ any authorization to Kafka as it assumes it's running in a secure network, e.g. private shared VNet. Consumer implementation is basic and doesn't support batching or concurrent processing. Dead letter and retry queues were not introduced. To optimize message size, custom JsonSerializerOptions with DefaultIgnoreCondition is used.

## Building blocks

Genereally not recommended practice to reuse code between microservices, but for such widely used dependencies as Event hub it was decided to introduce shared Building Block folder.

## Pixel.Service

The Pixel service is kept simple by defining the entire configuration and processing pipeline in Program.cs without splitting it into separate classes. 

Further clarification is required on the following aspects:
- ~~To reduce additional server IO, costs, and traffic, we could utilize public CDN services. The decision should be based on specific requirements as this introduces an additional initial round trip, but it would enable us to both process every /track request without caching and cache locally on customers machines image responses.
The implementation would be simple, replacing FileResult with a CDN url pointing to the file.~~ UPD: After reviewing other known implementations, it appears that all services return the image in the response.
- Currently, caching for the endpoint is disabled to ensure that every subsequent request is processed and stored. In-memory caching is utilized to load the image only once to reduce amount of IO operations and improve response time.

CORS were enabled to be able to access to website from other websites where img tag will be placed.

The IP address should be a mandatory value, but potential empty values are still sent to the Event hub topic for analysis purposes.

To improve availability, service won't wait for Event hub aknowledge of the message. Messages will be batched locally on the server and sent asynchronously, it will improve availability, but may lead to lost events.

## Storage.Worker

Given that public access is not required for the storage service, a Worker type of instance was chosen. It utilizes a smaller runtime Docker base image for improved efficiency.

The Storage service is kept simple by not introducing consumers at the application layer, and instead defining consume methods directly in the pipeline. An Onion architecture skeleton is set up and can be further developed later.

## Instructions

To run solution following instructions can be used:

```bash
# Go into the folder with solution and run:
$ docker-compose up
```

To override any settings, environment variables prefixed with TRACKER__ can be added to a .env file. For example:

```bash
TRACKER__KAFKA__SERVERS=["host.docker.internal:29092"]
TRACKER__FILEEVENTSTORE_FILE=/tmp/events.log
```

## Verification

Kafka
![Alt text for screenshot 1](/docs/Kafka.png)

EventStore
![Alt text for screenshot 2](/docs/EventStore.png)
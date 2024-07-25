# Reservations API

Hi! I'm Brandon and here's my attempt at the reservations API coding challenge.

## Getting started

### Step 1 - SQL Server Connection
To kick things off you'll need an instance of SQL Server. I've included a pre-configured [docker-compose](./docker-compose.yml) to get started with a basic instance of SQL Server. The connection string found in the API project's [appsettings.config](./API/appsettings.json) matches the connection information the container will be spun up with. 

🚨 **Important:** If you're already running SQL Server locally you will likely need to adjust the connection string information in [appsettings.config](./API/appsettings.json)

### Step 2 - Migrations
After you've verified your SQL Server connection, you'll want to run the [Entity Framework migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli). If you've got the proper tooling installed locally, you can execute the migrations with the following command:
```
dotnet ef database update
```

The migration will also seed the Database with some sample data. The extent of this data can be found in the OnModelCreating() override in the [ReservationDbContext class](./Services/ReservationDbContext.cs)

### Step 3 - API Endpoints
There are four API Endpoints that make up this application. To communicate with any of the endpoints you will need a valid, unexpired API Key associated with a User in the ApiKeys table. For simplicity these are all unencrypted plain-text GUIDS to be sent as Bearer tokens in the Authentication Header of the HTTP request.

1. HTTP GET `/api/appointment/open`: This endpoint is open to Clients or Providers and will return a **paged** list of available appointments ordered by appointment date. Users can provide search criteria(as query string parameters, check out the [RequestDTO](./Services/DTOs/GetAvailableAppointmentsRequestDTO.cs) for this endpoint for more detail) composed of a range of dates(up to seven consecutive days) and\or a Provider name if they're looking for someone specific.

2. HTTP POST `/api/availability/submit`: This endpoint allows Providers to submit their availability in blocks of time up to seven days. Here is an example request:
```json
{
  "startAvailabilityDate": "2024-07-28T10:21:09.379Z",
  "endAvailabilityDate": "2024-07-28T11:21:09.379Z"
}
```
Appointments must be 15 minutes in length and must start at :00, :15, :30: or :45. 

If a provider submits availability that does not start cleanly on a segmented time slot, the first appointment is created by rounding up to the nearest segment start after (so they don't start early than they submitted time for).

Likewise, if a provider submits availability that does not end cleanly on a segment, the last appointment will begin by rounding down to the nearest segment before (so they are not booked later than they submitted availability for).

3. HTTP POST `/api/appointment/reserve/{appointmentId}`: This endpoint allows clients to reserve an appointment slot. The reservation must be confirmed within 30 minutes or else it will become available for another client. Clients cannot book two appointments at the same time slot.

4. HTTP POST `/api/appointment/confirm/{appointmentId}`: This endpoint allows clients to confirm a previously reserved appointment within 30 minutes of reservation.

#### Postman Collection
You can find a simple Postman collection that demonstrates these four endpoints using the seeded data at [HM Reservations Requests.postman_collection.json](./HM%20Reservations%20Requests.postman_collection.json)

## Design Decisions
1. For simplicity a working User creation and management system has been omitted. However, I felt it was necessary to ensure the API data contracts look as they might in a production environment (i.e. without passing things like userIds and roles in the requests). For this purpose, I decided a rudimentary API Key system was in order. This primitive RBAC system works in conjunction with the [ApiKeyAuthorizeAttribute](./API/Attributes/ApiKeyAuthorizeAttribute.cs) to determine who a [User](./Services/Models/User.cs) is when making requests and what [Roles](./Services/Models/RoleEnum.cs) they can perform.

2. Appointment slots must start at :00, :15, :30, or :45. This makes things not only simpler for this example, but I would imagine most real users would prefer to interact with a system such as this.

3. Time Zones. Every developer's worst enemy. I opted not to fight my ancient adversary this time and used UTC across the board. In real life we don't get so lucky. 

## To Be Improved Upon Over Time
This section section includes things I wish I had more time for or would improve upon before sending to a production environment.
1. **TESTS TESTS TESTS** There are many complicated scenarios that can arise especially given the requirements of the system. I manually tested the "happy path" along with some basic scenarios, but I'd love to see the seed data being put to greater use with enhanced automated testing. I believe I covered *most* of these scenarios with validation and clever queries, but without the testing data to back it up, it's unknown. Such as:
    - Providers attempting to submit availability in the past
    - Providers attempting to submit a large quantity of availability
    - Providers and Admin should be able to be Clients and reserve appointments with a single user who has multiple roles
    - Providers can submit their availability for a given time slot idempotently
    - Clients should not be able to confirm a reservation they didn't make
    - Clients should not be able to confirm reservations made over thirty minutes in the past
    - Clients should not be able to reserve appointments with less than 24 hours notice
    - No expired keys should return data
    - Many more
    - A special note should be made about the use of DateTime.UtcNow in many places killing testing determinism. Those should be refactored to allow for deterministic tests.
2. Better Swagger Documentation
    - Swagger gets automatically generated and does include things like possible required\optional parameters, but more documentation around how to use this API would be ideal.
3. Deletion
    - Tables should include soft delete columns and API endpoints should be included to support user management in this regard
    - Continuing from the last point. More functionality should be provided to "undo" things users want undone. Such as allowing providers to retract availability. Clients should be allowed to cancel or move an appointment. Admin should be able to do anything to help customers with unique situations or when data gets in funky spots.
4. User management
    - This was somewhat mentioned earlier, but this project was designed as an API without a front end. Ideally users should be able to sign in to a website and be issued a JWT token and that token will grant them access to the platform. 
    - API keys should be user managed and able to be easily cycled
    - Inactive users should have inactive API keys
5. Indexes and query optimization
    - Some of the LINQ queries got a little intense. A closer eye should be kept on these queries over time to optimize performance. 
6. Boilerplate in controller methods
    - There was a bit too much boilerplate code in the the controller methods. Which leads me to my next point
7. Data validation is all over the place
    - In some places, user data validation was able to be performed without DB calls, so it was added in the RequestDTOs. In other places, validation required DB calls and so was placed in the service. Others were baked into the LINQ queries. There's no consistent way to get this data validation feedback back up to the user, which might make the API awkward to use.
8. Preventing abuse
    - A malicious actor could continuously reserve many time slots and do nothing with them
9. Logging and monitoring
    - A real production system needs to close the feedback loop back to developers by letting them know how their code is performing in the real world. 


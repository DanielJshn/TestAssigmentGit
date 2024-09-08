# Tasks API

This is an API that allows users to create accounts and manage their tasks.

## Setup

1. Clone the git repository: https://github.com/DanielJshn/TestAssigmentGit.git
2. Open the project in a code editor
3. Open the project in terminal
4. Restore the libraries with this command: `dotnet restore`
5. Change the connection string for MSSQL in the `appsettings.json`  file
6. Run the migrations to setup DB schema with this command: `dotnet tool install --global dotnet-ef`
7. Apply migrations with this command: `dotnet ef database update`
8. Build the project with this command: `dotnet build`
9. Run the project with this command: `dotnet watch run`

## Api Documentation

There are 2 features: Authentication and Tasks management.


### Authentication


To create a new user call POST `/users/register`, and pass such a request body:

```
{
  "name": "name",
  "email": "email@email.com",
  "password": "Pass123$"
}
```

The validation rules:
- name is not empty and has more than 3 characters
- email is valid
- password has ta least one digit, one letter and is longer than 8 characters

Successful response:
```
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3QyQGdtYWlsLmNvbSIsIm5iZiI6MTcyNTgxNjY4NywiZXhwIjoxNzI4NDA4Njg3LCJpYXQiOjE3MjU4MTY2ODd9.FeaFojeNQIc3pohlmXe76Ro_NcLZJ899-KnLndJX45A"
  }
}
```
--------------------------------------------------------------------------------

To login, call POST `/users/login` endpoint. In the request body you can use either the user name and password or email and password:

```
{
  "email": "test2@gmail.com",
  "password": "Pass123$"
}
```

Successful response:
```
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3QyQGdtYWlsLmNvbSIsIm5iZiI6MTcyNTgxNjY4NywiZXhwIjoxNzI4NDA4Njg3LCJpYXQiOjE3MjU4MTY2ODd9.FeaFojeNQIc3pohlmXe76Ro_NcLZJ899-KnLndJX45A"
  }
}
```


### Task Management

Tasks have `status` and `priority` parameters that have fixed values. For status: `Pending`, `InProgress`, `Completed`. For priority: `Low`, `Medium`, `High`.

To create a task, call POST `/task` and pass such a request body:

```
{
  "title": "title",
  "description": "description",
  "dueDate": "2024-09-08T17:38:02.469Z",
  "status": "Pending",
  "priority": "Low"
}
```

The successful response:
```
{
    "success": true,
    "message": null,
    "data": {
        "id": "922873ef-2e4d-4809-8a92-236d3278a34d",
        "title": "title",
        "description": "description",
        "dueDate": "2024-09-08T17:38:02.469Z",
        "status": "Pending",
        "priority": "Low",
        "createdAt": "2024-09-08T17:44:02.4542206Z",
        "updatedAt": "2024-09-08T17:44:02.4543162Z"
    }
}
```
--------------------------------------------------------------------------------
To update the task call PUT `/task/{taskID}` and pass the updated task fields in the request body:

```
{
  "title": "title2",
  "description": "description",
  "dueDate": "2024-09-08T17:38:02.469Z",
  "status": "InProgress",
  "priority": "Low"
}
```

Successful response:
```
{
    "success": true,
    "message": null,
    "data": {
        "id": "922873ef-2e4d-4809-8a92-236d3278a34d",
        "title": "title2",
        "description": "description",
        "dueDate": "2024-09-08T17:38:02.469Z",
        "status": "InProgress",
        "priority": "Low",
        "createdAt": "2024-09-08T17:44:02.4542206",
        "updatedAt": "2024-09-08T17:48:27.0144714Z"
    }
}
```
--------------------------------------------------------------------------------
To get a task by id all GET `/task/{taskID}`. The successful response:

```
{
    "success": true,
    "message": null,
    "data": {
        "id": "922873ef-2e4d-4809-8a92-236d3278a34d",
        "title": "title2",
        "description": "description",
        "dueDate": "2024-09-08T17:38:02.469",
        "status": "InProgress",
        "priority": "Low",
        "createdAt": "2024-09-08T17:44:02.4542206",
        "updatedAt": "2024-09-08T17:48:27.0144714"
    }
}
```
--------------------------------------------------------------------------------
To get all tasks call GET `/task`. You can additionally pass some parameters in the request query: the `pageSize` and `pageIndex` to configure pagination. By default, `pageIndex` is 1 and `pageSize` is 10. Also, you can pass parameters for filtering by status or priority to get specific tasks.
A successful response:

```
{
    "success": true,
    "message": null,
    "data": {
        "items": [
            {
                "id": "ac68ebc2-981c-4656-9ab2-19ae6b9e6442",
                "title": "string",
                "description": "string",
                "dueDate": "2024-09-05T11:55:37.915",
                "status": "Completed",
                "priority": "High",
                "createdAt": "2024-09-05T13:18:49.0753232",
                "updatedAt": "2024-09-05T13:18:49.4"
            },
            {
                "id": "e5846591-48ae-4afa-a004-5c132fe33b66",
                "title": "ooooo2",
                "description": "fd",
                "dueDate": null,
                "status": "Pending",
                "priority": "Low",
                "createdAt": "2024-09-07T11:09:07.4980543",
                "updatedAt": "2024-09-07T11:11:48.6002942"
            }
        ],
        "pageIndex": 1,
        "totalPages": 5,
        "hasPreviousPage": false,
        "hasNextPage": true
    }
}
```
--------------------------------------------------------------------------------
To delete a task call DELETE `/task/{taskID}`

Successful response:
```
{
    "success": true,
    "message": null,
    "data": null
}
```

# Architecture

The API is written using ASP.NET core Web api. The project is divided in a few layers:
- Repository is the bottom level, it is responsible to deal with the DB; 
- Service contains the business logic, validation rules, checks and other logic;
- Controller is responsible for processing the requests.

There is also a folder `tests` that contains unit tests for teh API.

The response schema is flexible and allows to return the response data and error messages. Error messages look like this:
```
{
  "success": false,
  "message": "Password must be at least 8 characters long, contain at least one digit and one special character.",
  "data": null
}
```




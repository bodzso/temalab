# aspnet-core-3-registration-login-api

ASP.NET Core 3.1 - Simple API for User Management, Authentication and Registration

For documentation and instructions check out https://jasonwatmore.com/post/2019/10/14/aspnet-core-3-simple-api-for-authentication-registration-and-user-management

## How to register a new user

POST - http://localhost:60133/users/register

```json
{
    "username": "jason",
    "firstName": "Jason",
    "lastName": "Watmore",
    "email": "a@a.com",
	"password": "my-super-secret-password"
}
```

## How to authenticate a user

POST - http://localhost:60133/users/authenticate

```json
{
    "username": "jason",
    "password": "my-super-secret-password"
}
```

## How to make an authenticated request to retrieve all users

GET - http://localhost:60133/users

## Transaction create

POST - http://localhost:60133/transactions

```json
{
    "name":"Első kiadás",
    "amount":-500,
    "date":"2020-03-27T16:15:27.683829+01:00",
    "userId":1
}
```

## Category create

POST -  http://localhost:60133/categories

```json
{
    "name":"Étkezés",
    "userId":1
}
```

## Transaction custom endpoints

transactions/revenues

transactions/expenses

transactions/pending

transactions/latest

## Category custom endpoints

categories
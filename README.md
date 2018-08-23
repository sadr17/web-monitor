# Web monitor app

The web application which provides the opportunity for monitoring web pages on the Internet.
The application provides CRUD-operations for list of web pages. There is a middleware service which pings defined web pages and
shows their statuses on the separate column. The middleware service uses sockets for updating statuses subscribed clients.
In the current implementation, there is a hard-coded time interval to check a list of pages (5 secs).

In order to show a simple authorization based on roles, a user can change the role (Anonym, Admin) on the main page of the application. 
Thus, it allows performing add/edit operations for a list of web pages.

The app consists of two main parts: back-end (ASP.NET Core Web-API) and front-end (React.js). 
Back-end service works with PostgreSQL database.

Additionally, all described services can be executed in Docker containers.

# Commands:
 - Run web monitor application (path: ./devops/):
    docker-compose up -d
    
 - Stop web monitor application (path: ./devops/):
    docker-compose stop
    
 - Run tests of back-end service (path: ./back-end/):
    docker-compose -f docker-compose.tests.yml up --build

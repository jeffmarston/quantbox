
Schema Compare
==============

TODO:


API Structure:

POST /schema
POST /schema/Viking

body = { version: '2019.4', objects: [...] } 

GET versions
GET schema/Viking
GET schema/*2019.4

GET objects/Viking/pr_GetSomething
GET objects/*2019.4/pr_GetSomething




GET /schema?client="Viking"
GET /schema?version="2019.4"

GET /schema/Viking?object="pr_GetWithIt"
GET /schema?object="pr_GetWithIt"


JSON dump of checksums
GET clients/Viking/schema
GET standard/schema

JSON dump of checksums
GET schema/Viking
GET schema/*		

contents of a specific object
GET schema/Viking/pr_GetSomething
GET schema/*/pr_GetSomething


Pages:

Scheam dump:
Standard  |  Client panes

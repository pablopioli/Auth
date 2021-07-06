# Auth
Extensions and tools for the user autentication needs. Mainly extensions to OpenIddict.

**Work in progress**

## TokenServer

An token server to use in integration tests. Objectives:

* Embedded in single EXE
* Easy to configure with test users, applications and scopes
* In memory storage

The UI is based on (an euphemism for copied) on https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-iv-authorization-code-flow-3eh8

## MemoryStorage

An storage extension for OpenIddict that stores all information in memory with optional persistence in Json files. Use it in TokenServer or in other projects.

The code is based (an euphemism for copied) on
https://github.com/panoukos41/couchdb-openiddict

## MartenStorage

Soon. An storage extension for OpenIddict that stores all information in a database using Marten (https://martendb.io). 

## ConsoleApp

Quick tests and samples on how to authenticate your users.


# License
MIT
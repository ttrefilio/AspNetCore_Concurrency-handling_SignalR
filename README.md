# AspNetCore_Concurrency-handling_SignalR

In this project I have implemented a pessimistic lock on a local database using the real-time comunication provided by SignalR to inform users when a lock is released.

Instalation:

1 - On the Package Manager Console, select Test.Application as the default project and run the command: update-database -context IdentyContext
    This will create the local database as well as the tables for the user authentication.
    
2 - Still on the Package Manage Console, now select Test.Data as the default project and run the command: update-database -context AppDbContext
    This will create the Entity table and the Lock table.

3 - When running the application, sign in with two different users in two diferent browsers (it also works with a icognito window) to observe the applications behavior, locking, unlocking a the interactions whith the users.

4 - Even though authorization was not implemented in this demonstration, it's importantant to authenticate the users to make use of the lock.

5 - On the repository class (Test.Data/Repositories/LockRepository.cs) a lock expiry time of 20 seconds is set. Feel free to change it as you please.

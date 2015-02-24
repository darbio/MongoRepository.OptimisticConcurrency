MongoRepository.OptimisticConcurrency
===

Basic optimistic concurrency implemented on top of the [MongoRepository](http://mongorepository.codeplex.com/) CodePlex project.

Constraints:

* Collection name must be explicitly specified in the Entity (see tests).
* Does not do concurrency check on delete.
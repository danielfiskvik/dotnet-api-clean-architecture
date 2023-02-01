namespace ModularApp.Modules.Workspace.Domain.Interfaces;

/*
 * In Clean Architecture, a BaseEntity is a basic building block that represents a domain object or concept.
 * It is an abstract CLASS or INTERFACE that defines the minimal set of properties and methods that any concrete entity in the domain must have.
 *
 * These properties and methods typically include
 * - a unique identifier,
 * - validation logic,
 * - and behavior related to the entity's role in the domain.
 *
 * The BaseEntity serves as a foundation for all other entities in the domain, allowing them to share common behavior and facilitating code reuse.
 *
 * Example of properties:
 * - Unique identifier: A property that represents the unique identifier for an entity in the domain. This could be an integer ID, a UUID, or a combination of different fields that together form a unique key.
 * - Validation logic: Methods for validating the state of an entity, such as checking if required fields have been set or if values fall within a certain range.
 * - Equality: A method for comparing two entities to see if they are the same, based on their unique identifier.
 * - Timestamps: Properties that capture the creation and last update timestamps of the entity.
 * - Relationship with other entities: Properties that represent the relationships of the entity with other entities in the domain.
 * - Business logic: Methods that encapsulate the behavior of the entity in the domain.
 *
 * These properties could vary depending on the use-case, but the above are the common properties a BaseEntity class should have.
 */
public interface IEntity
{
    /*
     * Whether to use a UUID (Universally Unique Identifier) or an integer for the ID of an entity depends on the specific requirements of your application and your use case.
     *
     * Here are some factors to consider when making this decision:
     * - Scalability: UUIDs are longer than integers, which means they take up more storage space.
     *   can be a concern if you have a large number of entities or if you are working with a limited amount of storage.
     *   However, if your application is expected to scale to a large number of entities,
     *   UUIDs can be more suitable as they are globally unique and can help to avoid conflicts when merging data from different sources.
     * - Database performance: Since UUIDs are larger than integers, they may take up more space in indexes and primary keys, which can affect database performance.
     *   However, in practice, the difference in performance between UUIDs and integers is usually negligible, especially if you use a database engine that supports native UUID types.
     * - Security: UUIDs are more difficult to guess than integers, making them more secure against brute-force attacks and other forms of data tampering.
     *
     * Ease of use: Integers are a simple, easy-to-use data type and are widely supported by most programming languages and databases.
     * UUIDs, on the other hand, can be a bit more complex to work with and may require additional libraries or code to handle them properly.
     * 
     * Ultimately, it's important to weigh the pros and cons of each option and decide which one is the best fit for your application and use case.
     * If you are not sure, you can use an integer for the ID and use a UUID as an additional identifier for the entity.
     */
    Guid Id { get; set; }
}
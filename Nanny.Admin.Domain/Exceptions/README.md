# Domain Exceptions

This directory contains domain-specific exceptions that enforce business rules and provide clear error messages for domain operations.

## Exception Hierarchy

```
DomainException (abstract base)
├── DomainValidationException
├── InvalidEntityStateException
├── DuplicateEntityException
├── OrderStateTransitionException
└── EmptyOrderException
```

## Exception Types

### DomainException
Base abstract class for all domain exceptions. Provides a foundation for domain-specific error handling.

### DomainValidationException
Thrown when domain validation fails. Includes entity type context and structured error information.

**Usage:**
```csharp
throw new DomainValidationException("Customer", "Customer validation failed", errors);
throw new DomainValidationException("Product", "Product validation failed", errors);
```

### InvalidEntityStateException
Thrown when an entity is in an invalid state for a requested operation.

**Usage:**
```csharp
throw new InvalidEntityStateException("Order", orderId, currentState, requiredState);
throw new InvalidEntityStateException("Order", orderId, "Cannot modify completed order");
```

### DuplicateEntityException
Thrown when attempting to create or add a duplicate entity.

**Usage:**
```csharp
throw new DuplicateEntityException("OrderLine", "ProductId", productId);
```

### OrderStateTransitionException
Thrown when an invalid order state transition is attempted.

**Usage:**
```csharp
throw new OrderStateTransitionException(orderId, currentStatus, requestedStatus);
throw new OrderStateTransitionException(orderId, currentStatus, requestedStatus, "Custom reason");
```

### EmptyOrderException
Thrown when attempting to complete an order that has no order lines.

**Usage:**
```csharp
throw new EmptyOrderException(orderId);
```

## Benefits

1. **Simple and Clean**: Minimal exception hierarchy that's easy to understand and maintain.

2. **Entity Context**: `DomainValidationException` includes entity type for clear error context.

3. **Structured Errors**: Validation exceptions provide structured error information for APIs.

4. **Domain-Specific Logic**: Business rule exceptions are tied to specific domain concepts.

5. **Better Error Handling**: Applications can catch specific exception types appropriately.

6. **Audit Trail**: Exceptions include relevant entity information for debugging and logging.

## Design Principles

- **Keep it Simple**: Minimal number of exception types
- **Entity Context**: Include entity type in validation exceptions
- **Structured Data**: Provide structured error information for APIs
- **Clear Messages**: Descriptive error messages for developers and users
- **Consistent Patterns**: Use consistent exception patterns across the domain

## Migration from Generic Exceptions

The domain entities have been updated to use these domain-specific exceptions instead of generic `ArgumentException` and `InvalidOperationException`:

- **Before**: `throw new ArgumentException("Name cannot be null or empty.", nameof(name));`
- **After**: `throw new DomainValidationException("Customer", "Customer validation failed", errors);`

- **Before**: `throw new InvalidOperationException("Order cannot be modified.");`
- **After**: `throw new InvalidEntityStateException("Order", Id, Status.ToString(), "modifiable state");`

## API Error Handling

The error handling middleware provides appropriate HTTP status codes:

- **400 Bad Request**: For validation failures
- **409 Conflict**: For state conflicts and duplicates
- **500 Internal Server Error**: For unexpected errors

Each validation exception returns structured error details including the entity type and specific validation errors. 
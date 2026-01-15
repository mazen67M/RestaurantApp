namespace RestaurantApp.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6,
    Rejected = 7
}

public enum OrderType
{
    Delivery = 0,
    Pickup = 1
}

public enum PaymentMethod
{
    CashOnDelivery = 0,
    Online = 1 // Future
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public enum UserRole
{
    Customer = 0,
    Cashier = 1,
    Admin = 2,
    Driver = 3
}

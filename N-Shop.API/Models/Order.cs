namespace N_Shop.API.Models;

public enum OrderStatus{
    Pending,
    Confirmed, //Approved
    Shipped,
    Delivered, //Completed
    Canceled
}

public enum PaymentMethodType
{
    CashOnDelivery,
    Visa
    //CreditCard,PayPal,Stripe,
}
public class Order
{
    //order
    public int Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ShippedDate { get; set; }
    public decimal TotalPrice { get; set; }
    //payment
    public PaymentMethodType PaymentMethodType { get; set; }
    public string? SessionId { get; set; }
    public string? TransactionId { get; set; }
    //carrier
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    //relation
    public ApplicationUser ApplicationUser { get; set; }
    public string ApplicationUserId { get; set; }
}
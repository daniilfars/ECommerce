namespace Ordering.Domain;

                    // Ожидание, подтверждено, оплачено, отправлено, доставлено, отменено
public enum OrderStatus { Pending, Confirmed, Paid, Shipped, Delivered, Cancelled };
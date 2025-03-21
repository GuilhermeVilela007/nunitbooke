public class BookingModel {

    public string firstname;
    public string lastname;
    public int totalprice;
    public bool depositpaid;
    public BookingDates bookingdates;
    public string additionalneeds;
}

public class BookingDates {
    
    public string checkin;
    public string checkout;

    public BookingDates(string checkin, string checkout){
        this.checkin = checkin;
        this.checkout = checkout;
    }

}
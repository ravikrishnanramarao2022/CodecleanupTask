using System;
using System.Linq;
using System.Collections.Generic;

namespace FlightBooking.Core
{
    public class ScheduledFlight
    {
        private readonly string VERTICAL_WHITE_SPACE = Environment.NewLine + Environment.NewLine;
        private readonly string NEW_LINE = Environment.NewLine;
        private const string INDENTATION = "    ";

        public ScheduledFlight(FlightRoute flightRoute)
        {
            FlightRoute = flightRoute;
            Passengers = new List<Passenger>();
        }

        public FlightRoute FlightRoute { get; private set; }
        public Plane Aircraft { get; private set; }

        public List<Passenger> Passengers { get; private set; }
        public bool IsAlternateRequried { get; private set; }

        public void AddPassenger(Passenger passenger)
        {
            Passengers.Add(passenger);
        }

        public void SetAircraftForRoute(Plane aircraft)
        {
            Aircraft = aircraft;
        }

        public string GetSummary()
        {
            double costOfFlight = 0;
            double profitFromFlight = 0;
            int totalLoyaltyPointsAccrued = 0;
            int totalLoyaltyPointsRedeemed = 0;
            int totalExpectedBaggage = 0;
            int seatsTaken = 0;
            int airLineEmployee = 0;
            bool rule1 = false;           
            IsAlternateRequried = false;

            string result = "Flight summary for " + FlightRoute.Title;

            foreach (var passenger in Passengers)
            {
                switch (passenger.Type)
                {
                    case (PassengerType.General):
                        {
                            profitFromFlight += FlightRoute.BasePrice;
                            totalExpectedBaggage++;
                            break;
                        }
                    case (PassengerType.LoyaltyMember):
                        {
                            if (passenger.IsUsingLoyaltyPoints)
                            {
                                int loyaltyPointsRedeemed = Convert.ToInt32(Math.Ceiling(FlightRoute.BasePrice));
                                passenger.LoyaltyPoints -= loyaltyPointsRedeemed;
                                totalLoyaltyPointsRedeemed += loyaltyPointsRedeemed;
                            }
                            else
                            {
                                totalLoyaltyPointsAccrued += FlightRoute.LoyaltyPointsGained;
                                profitFromFlight += FlightRoute.BasePrice;
                            }
                            totalExpectedBaggage += 2;
                            break;
                        }
                    case (PassengerType.AirlineEmployee):
                        {
                            totalExpectedBaggage += 1;
                            airLineEmployee += 1;
                            break;
                        }
                    case (PassengerType.Discounted):
                        {
                            profitFromFlight += (FlightRoute.BasePrice) / 2;
                            break;
                        }
                }
                costOfFlight += FlightRoute.BaseCost;
                seatsTaken++;
            }

            result += VERTICAL_WHITE_SPACE;

            result += "Total passengers: " + seatsTaken;
            result += NEW_LINE;
            result += INDENTATION + "General sales: " + Passengers.Count(p => p.Type == PassengerType.General);
            result += NEW_LINE;
            result += INDENTATION + "Loyalty member sales: " + Passengers.Count(p => p.Type == PassengerType.LoyaltyMember);
            result += NEW_LINE;
            result += INDENTATION + "Airline employee comps: " + Passengers.Count(p => p.Type == PassengerType.AirlineEmployee);
            result += NEW_LINE;
            result += INDENTATION + "Discounted: " + Passengers.Count(p => p.Type == PassengerType.Discounted);

            result += VERTICAL_WHITE_SPACE;
            result += "Total expected baggage: " + totalExpectedBaggage;

            result += VERTICAL_WHITE_SPACE;

            result += "Total revenue from flight: " + profitFromFlight;
            result += NEW_LINE;
            result += "Total costs from flight: " + costOfFlight;
            result += NEW_LINE;

            double profitSurplus = profitFromFlight - costOfFlight;

            result += (profitSurplus > 0 ? "Flight generating profit of: " : "Flight losing money of: ") + profitSurplus;

            result += VERTICAL_WHITE_SPACE;

            result += "Total loyalty points given away: " + totalLoyaltyPointsAccrued + NEW_LINE;
            result += "Total loyalty points redeemed: " + totalLoyaltyPointsRedeemed + NEW_LINE;

            result += VERTICAL_WHITE_SPACE;

            if ((airLineEmployee / (double)Aircraft.NumberOfSeats) > FlightRoute.MinimumTakeOffPercentage)
            {
                rule1 = true;
            }            
            if (rule1 || profitSurplus > 0 &&
                 seatsTaken < Aircraft.NumberOfSeats &&
                 seatsTaken / (double)Aircraft.NumberOfSeats > FlightRoute.MinimumTakeOffPercentage)
            {
                result += "This "+ Aircraft.Name +  "MAY PROCEED";
            }

            else
            {
                result += "This " + Aircraft.Name + " MAY NOT PROCEED" + NEW_LINE;
                IsAlternateRequried = true;
            }
            return result;
        }

        public string GetAlternativeSummary()
        {
            string result = "Other more suitable aircraft are: " + NEW_LINE;
            result += Aircraft.Name + " Could handle this flight";
            return result;

        }

    }
    
}

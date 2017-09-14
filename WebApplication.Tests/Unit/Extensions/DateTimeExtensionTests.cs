using System;
using System.Globalization;
using System.Threading;
using K9.Base.WebApplication.Extensions;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Extensions
{
	public class DateTimeExtensionTests
	{

		private DateTime _justNow = DateTime.Now.Subtract(TimeSpan.FromSeconds(59));
		private DateTime _minuteAgo = DateTime.Now.Subtract(TimeSpan.FromSeconds(74));
		private DateTime _2MinutesAgo = DateTime.Now.Subtract(TimeSpan.FromSeconds(120));
		private DateTime _59MinutesAgo = DateTime.Now.Subtract(TimeSpan.FromMinutes(59).Add(TimeSpan.FromSeconds(50)));
		private DateTime _hourAgo = DateTime.Now.Subtract(TimeSpan.FromHours(1));
		private DateTime _hourAgo59 = DateTime.Now.Subtract(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(59)));
		private DateTime _twoHoursAgo = DateTime.Now.Subtract(TimeSpan.FromHours(2));
		private DateTime _twoHoursAgo59 = DateTime.Now.Subtract(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(59)));
		private DateTime _23HoursAgo = DateTime.Now.Subtract(TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59)));
		private DateTime _yesterday = DateTime.Now.Subtract(TimeSpan.FromHours(24));
		private DateTime _yesterday23 = DateTime.Now.Subtract(TimeSpan.FromHours(24).Add(TimeSpan.FromHours(23)));
		private DateTime _2DaysAgo = DateTime.Now.Subtract(TimeSpan.FromHours(48));
		private DateTime _2DaysAgo23 = DateTime.Now.Subtract(TimeSpan.FromHours(48).Add(TimeSpan.FromHours(23)));
		private DateTime _6DaysAgo23 = DateTime.Now.Subtract(TimeSpan.FromDays(6).Add(TimeSpan.FromHours(23)).Subtract(TimeSpan.FromMinutes(59)));
	    private DateTime _1WeekAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));
	    private DateTime _8DaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(8));
	    private DateTime _13DaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(13).Add(TimeSpan.FromHours(23)));
	    private DateTime _2WeeksAgo = DateTime.Now.Subtract(TimeSpan.FromDays(14));
	    private DateTime _15DaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(15).Add(TimeSpan.FromHours(23)));
	    private DateTime _20DaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(20).Add(TimeSpan.FromHours(23)));
	    private DateTime _3WeeksAgo = DateTime.Now.Subtract(TimeSpan.FromDays(21));
	    private DateTime _27DaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(27));
        private DateTime _1MonthAgo = DateTime.Now.AddMonths(-1);
	    private DateTime _6MonthAgo = DateTime.Now.AddMonths(-6);
	    private DateTime _11MonthAgo = DateTime.Now.AddMonths(-11);
	    private DateTime _1YearAgo = DateTime.Now.AddYears(-1);
	    private DateTime _18MonthsAgo = DateTime.Now.AddMonths(-18);
	    private DateTime _18MonthsAgo23 = DateTime.Now.AddMonths(-23);
	    private DateTime _2YearsAgo = DateTime.Now.AddYears(-2);
	    private DateTime _9YearsAgo = DateTime.Now.AddYears(-9);
	    private DateTime _88YearsAgo = DateTime.Now.AddYears(-88);


        private DateTime _shortly = DateTime.Now.Add(TimeSpan.FromSeconds(59));
        private DateTime _minuteFuture = DateTime.Now.Add(TimeSpan.FromSeconds(74));
        private DateTime _2MinutesFuture = DateTime.Now.Add(TimeSpan.FromSeconds(125));
        private DateTime _59MinutesFuture = DateTime.Now.Add(TimeSpan.FromMinutes(59).Add(TimeSpan.FromSeconds(50)));
        private DateTime _hourFuture = DateTime.Now.Add(TimeSpan.FromHours(1)).AddSeconds(20);
        private DateTime _hourFuture59 = DateTime.Now.Add(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(59)));
        private DateTime _twoHoursFuture = DateTime.Now.Add(TimeSpan.FromHours(2)).AddSeconds(20);
        private DateTime _twoHoursFuture59 = DateTime.Now.Add(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(59)));
        private DateTime _23HoursFuture = DateTime.Now.Add(TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59)));
        private DateTime _tomorrow = DateTime.Now.Add(TimeSpan.FromHours(24)).AddSeconds(20);
        private DateTime _tomorrow23 = DateTime.Now.Add(TimeSpan.FromHours(24).Add(TimeSpan.FromHours(23)));
        private DateTime _2DaysFuture = DateTime.Now.Add(TimeSpan.FromHours(48)).AddSeconds(20);
        private DateTime _2DaysFuture23 = DateTime.Now.Add(TimeSpan.FromHours(48).Add(TimeSpan.FromHours(23)));
        private DateTime _6DaysFuture23 = DateTime.Now.Add(TimeSpan.FromDays(6).Add(TimeSpan.FromHours(23)).Add(TimeSpan.FromMinutes(59)));
        private DateTime _1WeekFuture = DateTime.Now.Add(TimeSpan.FromDays(7)).AddSeconds(20);
        private DateTime _8DaysFuture = DateTime.Now.Add(TimeSpan.FromDays(8)).AddSeconds(20);
        private DateTime _13DaysFuture = DateTime.Now.Add(TimeSpan.FromDays(13).Add(TimeSpan.FromHours(23)));
        private DateTime _2WeeksFuture = DateTime.Now.Add(TimeSpan.FromDays(14)).AddSeconds(20);
        private DateTime _15DaysFuture = DateTime.Now.Add(TimeSpan.FromDays(15).Add(TimeSpan.FromHours(23)));
        private DateTime _20DaysFuture = DateTime.Now.Add(TimeSpan.FromDays(20).Add(TimeSpan.FromHours(23)));
        private DateTime _3WeeksFuture = DateTime.Now.Add(TimeSpan.FromDays(21)).AddSeconds(20);
        private DateTime _27DaysFuture = DateTime.Now.Add(TimeSpan.FromDays(27)).AddSeconds(20);
        private DateTime _1MonthFuture = DateTime.Now.AddMonths(1).AddMinutes(1);
        private DateTime _6MonthFuture = DateTime.Now.AddMonths(6).AddSeconds(20);
        private DateTime _11MonthFuture = DateTime.Now.AddMonths(11).AddSeconds(20);
        private DateTime _1YearFuture = DateTime.Now.AddYears(1).AddSeconds(20);
        private DateTime _18MonthsFuture = DateTime.Now.AddMonths(18).AddSeconds(20);
        private DateTime _18MonthsFuture23 = DateTime.Now.AddMonths(23).AddSeconds(20);
        private DateTime _2YearsFuture = DateTime.Now.AddYears(2).AddSeconds(20);
        private DateTime _9YearsFuture = DateTime.Now.AddYears(9).AddSeconds(20);
        private DateTime _88YearsFuture = DateTime.Now.AddYears(88).AddSeconds(20);

        [Fact]
		public void ToHumanReadableString_ShouldDisplayCorrectValue()
		{
		    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            Assert.Equal("Just now", _justNow.ToHumanReadableString());
			Assert.Equal("1 minute ago", _minuteAgo.ToHumanReadableString());
			Assert.Equal("2 minutes ago", _2MinutesAgo.ToHumanReadableString());
			Assert.Equal("59 minutes ago", _59MinutesAgo.ToHumanReadableString());
			Assert.Equal("1 hour ago", _hourAgo.ToHumanReadableString());
			Assert.Equal("1 hour ago", _hourAgo59.ToHumanReadableString());
			Assert.Equal("2 hours ago", _twoHoursAgo.ToHumanReadableString());
			Assert.Equal("2 hours ago", _twoHoursAgo59.ToHumanReadableString());
			Assert.Equal("23 hours ago", _23HoursAgo.ToHumanReadableString());
			Assert.Equal("Yesterday", _yesterday.ToHumanReadableString());
			Assert.Equal("Yesterday", _yesterday23.ToHumanReadableString());
			Assert.Equal("2 days ago", _2DaysAgo.ToHumanReadableString());
			Assert.Equal("2 days ago", _2DaysAgo23.ToHumanReadableString());
			Assert.Equal("6 days ago", _6DaysAgo23.ToHumanReadableString());
		    Assert.Equal("1 week ago", _1WeekAgo.ToHumanReadableString());
		    Assert.Equal("8 days ago", _8DaysAgo.ToHumanReadableString());
		    Assert.Equal("13 days ago", _13DaysAgo.ToHumanReadableString());
		    Assert.Equal("2 weeks ago", _2WeeksAgo.ToHumanReadableString());
		    Assert.Equal("15 days ago", _15DaysAgo.ToHumanReadableString());
		    Assert.Equal("20 days ago", _20DaysAgo.ToHumanReadableString());
            Assert.Equal("3 weeks ago", _3WeeksAgo.ToHumanReadableString());
		    Assert.Equal("1 month ago", _1MonthAgo.ToHumanReadableString());
		    Assert.Equal("27 days ago", _27DaysAgo.ToHumanReadableString());
		    Assert.Equal("6 months ago", _6MonthAgo.ToHumanReadableString());
            Assert.Equal("11 months ago", _11MonthAgo.ToHumanReadableString());
		    Assert.Equal("1 year ago", _1YearAgo.ToHumanReadableString());
		    Assert.Equal("18 months ago", _18MonthsAgo.ToHumanReadableString());
		    Assert.Equal("18 months ago", _18MonthsAgo23.ToHumanReadableString());
            Assert.Equal("2 years ago", _2YearsAgo.ToHumanReadableString());
            Assert.Equal("9 years ago", _9YearsAgo.ToHumanReadableString());
            Assert.Equal("88 years ago", _88YearsAgo.ToHumanReadableString());
            
            Assert.Equal("Shortly", _shortly.ToHumanReadableString());
            Assert.Equal("In 1 minute", _minuteFuture.ToHumanReadableString());
            Assert.Equal("In 2 minutes", _2MinutesFuture.ToHumanReadableString());
            Assert.Equal("In 59 minutes", _59MinutesFuture.ToHumanReadableString());
            Assert.Equal("In 1 hour", _hourFuture.ToHumanReadableString());
            Assert.Equal("In 1 hour", _hourFuture59.ToHumanReadableString());
            Assert.Equal("In 2 hours", _twoHoursFuture.ToHumanReadableString());
            Assert.Equal("In 2 hours", _twoHoursFuture59.ToHumanReadableString());
            Assert.Equal("In 23 hours", _23HoursFuture.ToHumanReadableString());
            Assert.Equal("Tomorrow", _tomorrow.ToHumanReadableString());
            Assert.Equal("Tomorrow", _tomorrow23.ToHumanReadableString());
            Assert.Equal("In 2 days", _2DaysFuture.ToHumanReadableString());
            Assert.Equal("In 2 days", _2DaysFuture23.ToHumanReadableString());
            Assert.Equal("In 6 days", _6DaysFuture23.ToHumanReadableString());
            Assert.Equal("In 1 week", _1WeekFuture.ToHumanReadableString());
            Assert.Equal("In 8 days", _8DaysFuture.ToHumanReadableString());
            Assert.Equal("In 13 days", _13DaysFuture.ToHumanReadableString());
            Assert.Equal("In 2 weeks", _2WeeksFuture.ToHumanReadableString());
            Assert.Equal("In 15 days", _15DaysFuture.ToHumanReadableString());
            Assert.Equal("In 20 days", _20DaysFuture.ToHumanReadableString());
            Assert.Equal("In 3 weeks", _3WeeksFuture.ToHumanReadableString());
            Assert.Equal("In 1 month", _1MonthFuture.ToHumanReadableString());
            Assert.Equal("In 27 days", _27DaysFuture.ToHumanReadableString());
            Assert.Equal("In 6 months", _6MonthFuture.ToHumanReadableString());
            Assert.Equal("In 11 months", _11MonthFuture.ToHumanReadableString());
            Assert.Equal("In 1 year", _1YearFuture.ToHumanReadableString());
            Assert.Equal("In 18 months", _18MonthsFuture.ToHumanReadableString());
            Assert.Equal("In 18 months", _18MonthsFuture23.ToHumanReadableString());
            Assert.Equal("In 2 years", _2YearsFuture.ToHumanReadableString());
            Assert.Equal("In 9 years", _9YearsFuture.ToHumanReadableString());
            Assert.Equal("In 88 years", _88YearsFuture.ToHumanReadableString());
        }

		[Fact]
		public void ToHumanReadableString_ShouldDisplayCorrectValueInFrench()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");

			Assert.Equal("A l'instant", _justNow.ToHumanReadableString());
			Assert.Equal("Il y a 1 minute", _minuteAgo.ToHumanReadableString());
			Assert.Equal("Il y a 2 minutes", _2MinutesAgo.ToHumanReadableString());
			Assert.Equal("Il y a 59 minutes", _59MinutesAgo.ToHumanReadableString());
			Assert.Equal("Il y a 1 heure", _hourAgo.ToHumanReadableString());
			Assert.Equal("Il y a 1 heure", _hourAgo59.ToHumanReadableString());
			Assert.Equal("Il y a 2 heures", _twoHoursAgo.ToHumanReadableString());
			Assert.Equal("Il y a 2 heures", _twoHoursAgo59.ToHumanReadableString());
			Assert.Equal("Il y a 23 heures", _23HoursAgo.ToHumanReadableString());
			Assert.Equal("Hier", _yesterday.ToHumanReadableString());
			Assert.Equal("Hier", _yesterday23.ToHumanReadableString());
			Assert.Equal("Il y a 2 jours", _2DaysAgo.ToHumanReadableString());
			Assert.Equal("Il y a 2 jours", _2DaysAgo23.ToHumanReadableString());
			Assert.Equal("Il y a 6 jours", _6DaysAgo23.ToHumanReadableString());
		    Assert.Equal("Il y a 1 semaine", _1WeekAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 8 jours", _8DaysAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 13 jours", _13DaysAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 2 semaines", _2WeeksAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 15 jours", _15DaysAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 20 jours", _20DaysAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 3 semaines", _3WeeksAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 1 mois", _1MonthAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 27 jours", _27DaysAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 6 mois", _6MonthAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 11 mois", _11MonthAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 1 an", _1YearAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 18 mois", _18MonthsAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 18 mois", _18MonthsAgo23.ToHumanReadableString());
		    Assert.Equal("Il y a 2 ans", _2YearsAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 9 ans", _9YearsAgo.ToHumanReadableString());
		    Assert.Equal("Il y a 88 ans", _88YearsAgo.ToHumanReadableString());

            Assert.Equal("Tout de suite", _shortly.ToHumanReadableString());
            Assert.Equal("Dans 1 minute", _minuteFuture.ToHumanReadableString());
            Assert.Equal("Dans 2 minutes", _2MinutesFuture.ToHumanReadableString());
            Assert.Equal("Dans 59 minutes", _59MinutesFuture.ToHumanReadableString());
            Assert.Equal("Dans 1 heure", _hourFuture.ToHumanReadableString());
            Assert.Equal("Dans 1 heure", _hourFuture59.ToHumanReadableString());
            Assert.Equal("Dans 2 heures", _twoHoursFuture.ToHumanReadableString());
            Assert.Equal("Dans 2 heures", _twoHoursFuture59.ToHumanReadableString());
            Assert.Equal("Dans 23 heures", _23HoursFuture.ToHumanReadableString());
            Assert.Equal("Demain", _tomorrow.ToHumanReadableString());
            Assert.Equal("Demain", _tomorrow23.ToHumanReadableString());
            Assert.Equal("Dans 2 jours", _2DaysFuture.ToHumanReadableString());
            Assert.Equal("Dans 2 jours", _2DaysFuture23.ToHumanReadableString());
            Assert.Equal("Dans 6 jours", _6DaysFuture23.ToHumanReadableString());
            Assert.Equal("Dans 1 semaine", _1WeekFuture.ToHumanReadableString());
            Assert.Equal("Dans 8 jours", _8DaysFuture.ToHumanReadableString());
            Assert.Equal("Dans 13 jours", _13DaysFuture.ToHumanReadableString());
            Assert.Equal("Dans 2 semaines", _2WeeksFuture.ToHumanReadableString());
            Assert.Equal("Dans 15 jours", _15DaysFuture.ToHumanReadableString());
            Assert.Equal("Dans 20 jours", _20DaysFuture.ToHumanReadableString());
            Assert.Equal("Dans 3 semaines", _3WeeksFuture.ToHumanReadableString());
            Assert.Equal("Dans 1 mois", _1MonthFuture.ToHumanReadableString());
            Assert.Equal("Dans 27 jours", _27DaysFuture.ToHumanReadableString());
            Assert.Equal("Dans 6 mois", _6MonthFuture.ToHumanReadableString());
            Assert.Equal("Dans 11 mois", _11MonthFuture.ToHumanReadableString());
            Assert.Equal("Dans 1 an", _1YearFuture.ToHumanReadableString());
            Assert.Equal("Dans 18 mois", _18MonthsFuture.ToHumanReadableString());
            Assert.Equal("Dans 18 mois", _18MonthsFuture23.ToHumanReadableString());
            Assert.Equal("Dans 2 ans", _2YearsFuture.ToHumanReadableString());
            Assert.Equal("Dans 9 ans", _9YearsFuture.ToHumanReadableString());
            Assert.Equal("Dans 88 ans", _88YearsFuture.ToHumanReadableString());
        }

	}
}

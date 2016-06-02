using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AQI.AQILabs.Kernel;
using AQI.AQILabs.Kernel.Adapters.MSSQL;

namespace QApp
{
    class SettingUtils
    {
        public static string KernelConnectString = "Data Source=s2112d1v9q.database.windows.net,1433;Network Library=DBMSSOCN;Initial Catalog=AQIKernel;User ID=aqi;Password=Capital!1234;MultipleActiveResultSets=True";
        public static MSSQLDataSetAdapter KernelDataAdapter = new MSSQLDataSetAdapter();
        public static string StrategyConnectString = "Data Source=s2112d1v9q.database.windows.net,1433;Network Library=DBMSSOCN;Initial Catalog=AQIStrategies;User ID=aqi;Password=Capital!1234;MultipleActiveResultSets=True";
        public static MSSQLDataSetAdapter StrategyDataAdapter = new MSSQLDataSetAdapter();
        public static void SetupCurrentUser()
        {
            AQI.AQILabs.Kernel.User.CurrentUser = new AQI.AQILabs.Kernel.User("System");
        }
        public static void SetupKernel()
        {
            if (!Database.DB.ContainsKey("Kernel"))
            {
                KernelDataAdapter.ConnectString = KernelConnectString;
                Database.DB.Add("Kernel", KernelDataAdapter);

                Calendar.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLCalendarFactory();
                Currency.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLCurrencyFactory();
                CurrencyPair.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLCurrencyPairFactory();
                DataProvider.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLDataProviderFactory();
                Exchange.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLExchangeFactory();
                Instrument.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLInstrumentFactory();
                Security.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLSecurityFactory();
                Future.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLFutureFactory();
                Portfolio.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLPortfolioFactory();
                Strategy.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLStrategyFactory();
                Market.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLMarketFactory();
                InterestRate.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLInterestRateFactory();
                Deposit.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLDepositFactory();
                InterestRateSwap.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLInterestRateSwapFactory();
                M.Factory = new AQI.AQILabs.Kernel.Adapters.MSSQL.Factories.MSSQLMFactory();
            }
        }
        public static void SetupDefaultStrategy()
        {
            if (!Database.DB.ContainsKey("DefaultStrategy"))
            {
                StrategyDataAdapter.ConnectString = StrategyConnectString;
                Database.DB.Add("DefaultStrategy", StrategyDataAdapter);
            }
        }

    }
}

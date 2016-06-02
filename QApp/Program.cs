using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AQI.AQILabs.Kernel;
using AQI.AQILabs.Kernel.Adapters.MSSQL;
using System.Data;

namespace QApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define variables
            int instrumentid, memoryclassid, memorytypeid, frequency, initdays;
            bool updateall;
            // Setup database connections
            SettingUtils.SetupCurrentUser();
            SettingUtils.SetupKernel();
            SettingUtils.SetupDefaultStrategy();
            // Get table from connection
            DatabaseUtils.GetIndicatorTickerTable();
            // Main loop
            int count = 0;
            foreach (DataRow DataRow in DatabaseUtils.IndicatorTickerTable.Rows)
            {
                instrumentid = (int)DataRow["KernelTickerID"];
                memoryclassid = (int)DataRow["TransformationID"];
                memorytypeid = (int)DataRow["RollTypeID"];
                frequency = (int)DataRow["FrequencyType"];
                updateall = true;
                initdays = 90;
                FunctionUtils.UpdateStrategyMemoryLevelScore(instrumentid, memoryclassid, memorytypeid, frequency, updateall, initdays);
                ++count;
                //if (count > 0)
                //{
                //    break;
                //}
            }
            Console.WriteLine("Press to continue...");
            Console.ReadKey();
        }
    }
}
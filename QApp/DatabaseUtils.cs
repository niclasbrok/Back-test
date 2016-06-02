using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AQI.AQILabs.Kernel;
using System.IO;
using System.Data;

namespace QApp
{
    public class DatabaseUtils
    {
        public static DataTable IndicatorTickerTable = null;
        public static DataTable LevelMemoryTable = null;
        public static void GetIndicatorTickerTable()
        {
            IndicatorTickerTable = Database.DB["DefaultStrategy"].GetDataTable("CMIndicatorTicker", null, "IsActive = 1");
        }
        public static void GetLevelMemoryTable(int instrumentid, int levelscoreid, int memoryclassid)
        {
            LevelMemoryTable = Database.DB["DefaultStrategy"].GetDataTable("StrategyMemory", null, "ID = " + instrumentid + " AND MemoryTypeID = " + levelscoreid + " AND MemoryClassID = " + memoryclassid);
        }
    }
}

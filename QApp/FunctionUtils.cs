using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AQI.AQILabs.Kernel;
using AQI.AQILabs.Kernel.Adapters.MSSQL;
using System.Data;
using System.Diagnostics;

namespace QApp
{
    public class FunctionUtils
    {
        private static Stopwatch Stopwatch = new Stopwatch(); 
        public static void UpdateStrategyMemoryLevelScore(int instrumentid, int memoryclassid, int memorytypeid, int frequency, bool updateall,int initdays)
        {
            int levelscoreid = 93;
            Instrument Instrument = Instrument.FindInstrument(instrumentid);
            AQI.AQILabs.Kernel.Numerics.Util.TimeSeries InstrumentTimeSeries = (Instrument as Strategy).GetMemorySeries(memorytypeid, memoryclassid);
            string dash = "--------------------------------------------------------------------";
            Console.WriteLine(dash + Environment.NewLine + "\t" + "\t" + Instrument.Name + " (" + instrumentid + "," + memoryclassid + "," + memorytypeid + ")" + Environment.NewLine + dash);
            Console.WriteLine("... Retrieving table ...");
            DatabaseUtils.GetLevelMemoryTable(instrumentid, levelscoreid, memoryclassid);
            Console.WriteLine("... Main loop ...");
            Console.WriteLine("Number of existing rows: " + DatabaseUtils.LevelMemoryTable.Rows.Count);
            Console.WriteLine("Rows to add: " + (InstrumentTimeSeries.Count - DatabaseUtils.LevelMemoryTable.Rows.Count));
            // Initialize parameters
            if (InstrumentTimeSeries.Count == 0)
            {
                Console.WriteLine("No Observations exist!");
                return;
            }
            double LocalVal, LevelScore;
            double LocalMin = InstrumentTimeSeries[0];
            double LocalMax = InstrumentTimeSeries[0];
            if (!updateall & DatabaseUtils.LevelMemoryTable.Rows.Count == 0)
            {
                // If the database is empty but we don't want to update everything -- we update everything
                Console.WriteLine("No levels exist - we update everything!");
                updateall = true;
            }
            if (updateall & DatabaseUtils.LevelMemoryTable.Rows.Count > 0)
            {
                // If we update everything and data exist - we clear the table
                Console.WriteLine("We want to update everything - we delete existing rows!"); ;
                string SqlDelete = "DELETE FROM StrategyMemory WHERE ID = " + instrumentid + " AND MemoryTypeID = " + levelscoreid + " AND MemoryclassID = " + memoryclassid;
                SettingUtils.StrategyDataAdapter.ExecuteCommand(SqlDelete);
            }
            if (updateall)
            {
                bool CompFlag = Math.Sign(instrumentid) == 1;
                int InitDataPoints = (int)Math.Ceiling((double)initdays/(double)frequency);
                Stopwatch.Start();
                for (int k = 0;k < InstrumentTimeSeries.Count; ++k)
                {
                    // Update min and max
                    LocalVal = InstrumentTimeSeries[k];
                    if (LocalVal < LocalMin){LocalMin = LocalVal;}
                    if (LocalVal > LocalMax){LocalMax = LocalVal;}
                    if (k % 100 == 0){Console.WriteLine("Processing date: " + InstrumentTimeSeries.DateTimes[k] + "\t Rows to add: " + (InstrumentTimeSeries.Count - k));}
                    if (k < InitDataPoints)
                    {
                        // Put the initial values to the average
                        LevelScore = 5.5;
                    }
                    else
                    {
                        // Use the 1-10 mapping on the remaining values - depending on high/low definition
                        if (CompFlag)
                        {
                            LevelScore = ((LocalVal - LocalMin) / (LocalMax - LocalMin)) * 9 + 1;
                        }
                        else
                        {
                            LevelScore = 9 - ((LocalVal - LocalMin) / (LocalMax - LocalMin)) * 9 + 1;
                        }
                    }
                    // Insert into database
                    DataRow NewRow = DatabaseUtils.LevelMemoryTable.NewRow();
                    NewRow["ID"] = instrumentid;
                    NewRow["MemoryTypeID"] = levelscoreid;
                    NewRow["TimeStamp"] = InstrumentTimeSeries.DateTimes[k];
                    if (LocalMin == LocalMax)
                    {
                        NewRow["Value"] = 5.5;
                    }
                    else
                    {
                        NewRow["Value"] = LevelScore;
                    }
                    NewRow["MemoryClassID"] = memoryclassid;
                    DatabaseUtils.LevelMemoryTable.Rows.Add(NewRow);
                }
                Stopwatch.Stop();
                Console.WriteLine("Time spend looping: " + (Stopwatch.ElapsedMilliseconds) + "ms");
                Console.WriteLine("... Appending to database ...");
                Stopwatch.Restart();
                Stopwatch.Start();
                Database.DB["DefaultStrategy"].UpdateDataTable(DatabaseUtils.LevelMemoryTable);
                Stopwatch.Stop();
                Console.WriteLine("Time spend appending: " + (Stopwatch.ElapsedMilliseconds) + "ms");
            }
            else
            {
                // Get values exceeding the last DateTime in the current LevelMemoryTable compared to new observation
                bool CompFlag = Math.Sign(instrumentid) == 1;
                AQI.AQILabs.Kernel.Numerics.Util.TimeSeries LevelTimeSeries = (Instrument as Strategy).GetMemorySeries(levelscoreid, memoryclassid);
                LocalMin = InstrumentTimeSeries.Values.Take(LevelTimeSeries.Count).Min();
                LocalMax = InstrumentTimeSeries.Values.Take(LevelTimeSeries.Count).Max();
                Stopwatch.Start();
                for (int k = LevelTimeSeries.Count;k < InstrumentTimeSeries.Count;++k)
                {
                    if (k % 100 == 0){Console.WriteLine("Processing date: " + InstrumentTimeSeries.DateTimes[k] + "\t Rows to add: " + (InstrumentTimeSeries.Count - k));}
                    LocalVal = InstrumentTimeSeries[k];
                    if (LocalVal < LocalMin){LocalMin = LocalVal;}
                    if (LocalVal > LocalMax){LocalMax = LocalVal;}
                    // Use the 1-10 mapping on the remaining values - depending on high/low definition
                    if (CompFlag)
                    {
                        LevelScore = ((LocalVal - LocalMin) / (LocalMax - LocalMin)) * 9 + 1;
                    }
                    else
                    {
                        LevelScore = 9 - ((LocalVal - LocalMin) / (LocalMax - LocalMin)) * 9 + 1;
                    }
                    // Insert into database
                    DataRow NewRow = DatabaseUtils.LevelMemoryTable.NewRow();
                    NewRow["ID"] = instrumentid;
                    NewRow["MemoryTypeID"] = levelscoreid;
                    NewRow["TimeStamp"] = InstrumentTimeSeries.DateTimes[k];
                    if (LocalMin == LocalMax)
                    {
                        NewRow["Value"] = 5.5;
                    }
                    else
                    {
                        NewRow["Value"] = LevelScore;
                    }
                    NewRow["MemoryClassID"] = memoryclassid;
                    DatabaseUtils.LevelMemoryTable.Rows.Add(NewRow);
                }
                Stopwatch.Stop();
                Console.WriteLine("Time spend looping: " + (Stopwatch.ElapsedMilliseconds) + "ms");
                Console.WriteLine("... Appending to database ...");
                Stopwatch.Restart();
                Stopwatch.Start();
                Database.DB["DefaultStrategy"].UpdateDataTable(DatabaseUtils.LevelMemoryTable);
                Stopwatch.Stop();
                Console.WriteLine("Time spend appending: " + (Stopwatch.ElapsedMilliseconds) + "ms");
            }
            Stopwatch.Restart();
        }
    }
}
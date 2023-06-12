﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Universal_x86_Tuning_Utility.Scripts.AMD_Backend;

namespace Universal_x86_Tuning_Utility.Scripts
{
    public class Family
    {
        public enum RyzenFamily
        {
            Unknown = -1,
            SummitRidge,
            PinnacleRidge,
            RavenRidge,
            Dali,
            Pollock,
            Picasso,
            FireFlight,
            Matisse,
            Renoir,
            Lucienne,
            VanGogh,
            Mendocino,
            Vermeer,
            Cezanne_Barcelo,
            Rembrandt,
            Raphael,
            DragonRange,
            PhoenixPoint,
            PhoenixPoint2,
            SonomaValley,
            GraniteRidge,
            FireRange,
            HawkPoint,
            StrixPoint,
            StrixPoint2,
        }

        public static RyzenFamily FAM = RyzenFamily.Unknown;

        public enum ProcessorType
        {
            Unknown = -1,
            Amd_Apu,
            Amd_Desktop_Cpu,
            Amd_Laptop_Cpu,
            Intel,
        }

        public static ProcessorType TYPE = ProcessorType.Unknown;


        public static string CPUName = "";
        public static int CPUFamily = 0, CPUModel = 0, CPUStepping = 0;
        public static async void setCpuFamily()
        {
            try
            {
                string processorIdentifier = System.Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

                // Split the string into individual words
                string[] words = processorIdentifier.Split(' ');

                // Find the indices of the words "Family", "Model", and "Stepping"
                int familyIndex = Array.IndexOf(words, "Family") + 1;
                int modelIndex = Array.IndexOf(words, "Model") + 1;
                int steppingIndex = Array.IndexOf(words, "Stepping") + 1;

                // Extract the family, model, and stepping values from the corresponding words
                CPUFamily = int.Parse(words[familyIndex]);
                CPUModel = int.Parse(words[modelIndex]);
                CPUStepping = int.Parse(words[steppingIndex].TrimEnd(','));

                ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                   CPUName = mo["Name"].ToString();
                }
            }
            catch (ManagementException e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }

            if (CPUName.Contains("Intel")) TYPE = ProcessorType.Intel;
            else
            {
                //Zen1 - Zen2
                if (CPUFamily == 23)
                {
                    if (CPUModel == 1) FAM = RyzenFamily.SummitRidge;

                    if (CPUModel == 8) FAM = RyzenFamily.PinnacleRidge;

                    if (CPUModel == 18) FAM = RyzenFamily.RavenRidge;

                    if (CPUModel == 24) FAM = RyzenFamily.Picasso;

                    if(CPUModel == 32 && CPUName.Contains("15e") || CPUModel == 32 && CPUName.Contains("15Ce") || CPUModel == 32 && CPUName.Contains("20e")) FAM = RyzenFamily.Pollock;
                    else if (CPUModel == 32) FAM = RyzenFamily.Dali;

                    if (CPUModel == 80) FAM = RyzenFamily.FireFlight;

                    if (CPUModel == 96) FAM = RyzenFamily.Renoir;

                    if (CPUModel == 104) FAM = RyzenFamily.Lucienne;

                    if (CPUModel == 113) FAM = RyzenFamily.Matisse;

                    if (CPUModel == 144) FAM = RyzenFamily.VanGogh;

                    if (CPUModel == 160) FAM = RyzenFamily.Mendocino;
                }

                //Zen3 - Zen4
                if (CPUFamily == 25)
                {
                    if (CPUModel == 33) FAM = RyzenFamily.Vermeer;

                    if (CPUModel == 63 || CPUModel == 68) FAM = RyzenFamily.Rembrandt;

                    if (CPUModel == 80) FAM = RyzenFamily.Cezanne_Barcelo;

                    if (CPUModel == 97 && CPUName.Contains("HX")) FAM = RyzenFamily.DragonRange;
                    else if (CPUModel == 97) FAM = RyzenFamily.Raphael;

                    if (CPUModel == 116) FAM = RyzenFamily.PhoenixPoint;
                }

                if (FAM == RyzenFamily.SummitRidge || FAM == RyzenFamily.PinnacleRidge || FAM == RyzenFamily.Matisse || FAM == RyzenFamily.Vermeer || FAM == RyzenFamily.Raphael) TYPE = ProcessorType.Amd_Desktop_Cpu;
                else if (FAM == RyzenFamily.DragonRange) TYPE = ProcessorType.Amd_Laptop_Cpu;
                else TYPE = ProcessorType.Amd_Apu;

                Addresses.setAddresses();
            }

            //Clipboard.SetText(System.Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER").ToString());
            //MessageBox.Show(CPUFamily.ToString() + " "  + FAM.ToString());
        }
    }
}

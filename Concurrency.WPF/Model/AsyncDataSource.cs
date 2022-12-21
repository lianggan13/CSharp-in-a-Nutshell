/*----------------------------------------------------------------
* Copyright (c) 2022 ZhangLiang All Rights Reserved
* Domain:   DESKTOP-GE6HNEQ
* Author：  ZhangLiang
* TEL:      15694047739
* WX:       GanNo-13
*----------------------------------------------------------------*/

using System.Threading;

namespace Concurrency.WPF.Model
{
    public class AsyncDataSource
    {
        private string _fastDP;
        private string _slowerDP;
        private string _slowestDP;

        public AsyncDataSource()
        {
        }

        public string FastDP
        {
            get
            {
                Thread.Sleep(4000);
                return _fastDP;
            }
            set { _fastDP = value; }
        }

        public string SlowerDP
        {
            get
            {
                // This simulates a lengthy time before the
                // data being bound to is actualy available.
                Thread.Sleep(7000);
                return _slowerDP;
            }
            set { _slowerDP = value; }
        }

        public string SlowestDP
        {
            get
            {
                // This simulates a lengthy time before the
                // data being bound to is actualy available.
                Thread.Sleep(10000);
                return _slowestDP;
            }
            set { _slowestDP = value; }
        }
    }
}

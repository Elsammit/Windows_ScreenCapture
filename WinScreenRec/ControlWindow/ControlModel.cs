using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScreenRec.ControlWindow
{
    class ControlModel
    {

        public void StartRecord()
        {
            CapAreaViewModel.m_CapAreaModel.CommunicationConfirm();
        }
    }
}

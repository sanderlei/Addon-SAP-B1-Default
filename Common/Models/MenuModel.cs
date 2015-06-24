using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Enums;

namespace Common.Models
{
    public class MenuModel
    {
        public MenuModel()
        { }

        private Boolean _bEnabled = true;

        private EnumMenuType _sMenuType;

        private Int32 _iPosition;

        private String _sDescription;
        private String _sParentId;
        private String _sUniqueId;
        private String _sImageName;

        public String Description
        {
            get { return this._sDescription; }
            set { this._sDescription = value; }
        }

        public String Parent
        {
            get { return this._sParentId; }
            set { this._sParentId = value; }
        }

        public String UniqueID
        {
            get { return this._sUniqueId; }
            set { this._sUniqueId = value; }
        }
        public EnumMenuType Type
        {
            get { return this._sMenuType; }
            set { this._sMenuType = value; }
        }
        public Int32 Position
        {
            get { return this._iPosition; }
            set { this._iPosition = value; }
        }

        public Boolean Enabled
        {
            get { return this._bEnabled; }
            set { this._bEnabled = value; }
        }

        public String ImageName
        {
            get { return this._sImageName; }
            set { this._sImageName = value; }
        }


    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronInstruments
{
    class PaneViewModel : ViewModelBase
    {
        public PaneViewModel()
        { }

        public virtual Uri IconSource
        {
            get
            {
                return null;
            }
        }

        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        #endregion

        #region ContentId

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    RaisePropertyChanged("ContentId");
                }
            }
        }

        #endregion


    }
}

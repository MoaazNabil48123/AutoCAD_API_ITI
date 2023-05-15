using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using final_Project.Autocad;
using final_Project.Command;
using final_Project.View;
using System.Collections.Generic;

namespace final_Project.ViewModel
{
    public class ColumnDim_VM : ViewModelBase
    {
        #region Fields
        private string _columnLayerName;
        private string _axesLayerName;
        private string _dimLayerName = "Column DIM";
        private string _SelectedUnit;
        private string _selectedDimStyle;

        #endregion

        #region Property
        public List<string> AllLayers { get; set; }
        public List<string> units { get; set; }
        public List<string> DimStyleList { get; set; }

        public string ColumnLayerName
        {
            get { return _columnLayerName; }
            set
            {
                _columnLayerName = value;
                OnPropertyChanged();
            }
        }
        public string SelectedUnit
        {
            get { return _SelectedUnit; }
            set
            {
                _SelectedUnit = value;
                OnPropertyChanged();
            }
        }


        public string AxesLayerName
        {
            get { return _axesLayerName; }
            set
            {
                _axesLayerName = value;
                OnPropertyChanged();
            }
        }

        public string DimLayerName
        {
            get { return _dimLayerName; }
            set
            {
                _dimLayerName = value;
                OnPropertyChanged();
            }
        }

        public string SelectedDimStyle
        {
            get { return _selectedDimStyle; }
            set
            {
                _selectedDimStyle = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands
        public MyCommand OkCommand { get; set; }
        #endregion

        #region Methods
        public void Ok(object parameter)
        {
            AutocadMethods.ColumnTag(_columnLayerName, _axesLayerName, _dimLayerName, SelectedUnit, SelectedDimStyle,false);

            (parameter as ColumnDim).Close();
        }
        #endregion

        #region Constructor
        public ColumnDim_VM()
        {
            #region LayersList & TextStyle List
            AllLayers = new List<string>();
            DimStyleList = new List<string>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                #region LayersList
                LayerTable LT = transaction.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (ObjectId ID in LT)
                {
                    AllLayers.Add((transaction.GetObject(ID, OpenMode.ForRead) as LayerTableRecord).Name);
                }
                #endregion

                #region DimStyle List
                DimStyleTable DST = transaction.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                foreach (ObjectId item in DST)
                {
                    DimStyleList.Add((transaction.GetObject(item, OpenMode.ForRead) as DimStyleTableRecord).Name);
                }
                #endregion
                transaction.Commit();
            }
            #endregion

            //unit List
            units = new List<string>() { "m", "cm", "mm" };

            OkCommand = new MyCommand(Ok);
        }
        #endregion

    }
}

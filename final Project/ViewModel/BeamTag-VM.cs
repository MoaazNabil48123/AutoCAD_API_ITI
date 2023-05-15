using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using final_Project.Autocad;
using final_Project.Command;
using final_Project.View;
using System.Collections.Generic;

namespace final_Project.ViewModel
{
    public class BeamTag_VM : ViewModelBase
    {
        #region Fields

        private string _firstLayer= "<---- None ------>";
        private string _secondLayer= "<---- None ------>";
        private string _thirdLayer= "<---- None ------>";
        private string _fourthLayer= "<---- None ------>";
        private string _fifthLayer= "<---- None ------>";
        List<string> LayersNames = new List<string>();

        private string _firstTag = "B1 250x600";
        private string _secondTag = "B2 250x700";
        private string _thirdTag = "B3 250x800";
        private string _fourthTag = "B4 250x900";
        private string _fifthTag = "B5 300x1000";
        List<string> tagsText = new List<string>();

        private string _tagLayerName = "Beam Tags";

        #endregion
        #region Property

        public List<string> AllLayers { get; set; }


        public string FirstLayer
        {
            get { return _firstLayer; }
            set
            {
                _firstLayer = value;
                OnPropertyChanged();
            }
        }
        public string SecondLayer
        {
            get { return _secondLayer; }
            set
            {
                _secondLayer = value;
                OnPropertyChanged();
            }
        }
        public string ThirdLayer
        {
            get { return _thirdLayer; }
            set
            {
                _thirdLayer = value;
                OnPropertyChanged();
            }
        }
        public string FourthLayer
        {
            get { return _fourthLayer; }
            set
            {
                _fourthLayer = value;
                OnPropertyChanged();
            }
        }
        public string FifthLayer
        {
            get { return _fifthLayer; }
            set
            {
                _fifthLayer = value;
                OnPropertyChanged();
            }
        }

        public string FirstTag
        {
            get { return _firstTag; }
            set
            {
                _firstTag = value;
                OnPropertyChanged();
            }
        }
        public string SecondTag
        {
            get { return _secondTag; }
            set
            {
                _secondTag = value;
                OnPropertyChanged();
            }
        }
        public string ThirdTag
        {
            get { return _thirdTag; }
            set
            {
                _thirdTag = value;
                OnPropertyChanged();
            }
        }
        public string FourthTag
        {
            get { return _fourthTag; }
            set
            {
                _fourthTag = value;
                OnPropertyChanged();
            }
        }
        public string FifthTag
        {
            get { return _fifthTag; }
            set
            {
                _fifthTag = value;
                OnPropertyChanged();
            }
        }

        public string TagLayerName
        {
            get { return _tagLayerName; }
            set
            {
                _tagLayerName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command
        public MyCommand OkCommand { get; set; }
        #endregion

        #region Method
        public void Ok(object parameter)
        {
            if (_firstLayer != null  && _firstLayer != "<---- None ------>") { LayersNames.Add(_firstLayer); }
            if (_secondLayer != null && _secondLayer != "<---- None ------>") { LayersNames.Add(_secondLayer); }
            if (_thirdLayer != null  && _thirdLayer != "<---- None ------>") { LayersNames.Add(_thirdLayer); }
            if (_fourthLayer != null && _fourthLayer != "<---- None ------>") { LayersNames.Add(_fourthLayer); }
            if (_fifthLayer != null  && _fifthLayer != "<---- None ------>") { LayersNames.Add(_fifthLayer); }

            if (_firstLayer != null && _firstLayer != "<---- None ------>") { tagsText.Add(_firstTag); }
            if (_secondLayer != null&& _secondLayer != "<---- None ------>") { tagsText.Add(_secondTag); }
            if (_thirdLayer != null && _thirdLayer != "<---- None ------>") { tagsText.Add(_thirdTag); }
            if (_fourthLayer != null&& _fourthLayer != "<---- None ------>") { tagsText.Add(_fourthTag); }
            if (_fifthLayer != null && _fifthLayer != "<---- None ------>") { tagsText.Add(_fifthTag); }

            double e = _firstTag.Length * 0.06;
            AutocadMethods.BeamTag(LayersNames, tagsText, TagLayerName,e);

            (parameter as BeamTag).Close();
        }
        #endregion

        #region Constructor
        public BeamTag_VM()
        {
            AllLayers = new List<string>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                LayerTable LT = transaction.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (ObjectId ID in LT)
                {
                    AllLayers.Add((transaction.GetObject(ID, OpenMode.ForRead) as LayerTableRecord).Name);
                }
                AllLayers.Add("<---- None ------>");
                transaction.Commit();
            }


            OkCommand = new MyCommand(Ok);
        }
        #endregion

    }
}

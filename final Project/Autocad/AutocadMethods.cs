using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using final_Project.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace final_Project.Autocad
{
    public class AutocadMethods
    {
        #region BeamTag
        public static void BeamTag(List<string> layersNames, List<string> tagsText, string tagLayerName, double e)
        {

            double constant = 0.4;
            //start the code
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DocumentLock dl = doc.LockDocument();
            Database db = doc.Database;
            Editor editor = doc.Editor;
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                try
                {
                    //creat new layer for beam tag if it's not existed
                    LayerTable layerTable = transaction.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    if (!(layerTable.Has(tagLayerName)))
                    {
                        LayerTableRecord tagLayer = new LayerTableRecord();
                        tagLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 3);
                        tagLayer.Name = tagLayerName;
                        layerTable.Add(tagLayer);
                        transaction.AddNewlyCreatedDBObject(tagLayer, true);
                    }

                    //loop for each layer to get the lines in it and carry out tag for them
                    for (int i = 0; i < layersNames.Count; i++)
                    {

                        //select all the lines at specific layer
                        TypedValue[] value = new TypedValue[2];
                        value[0] = new TypedValue((int)DxfCode.Start, "Line");
                        value[1] = new TypedValue((int)DxfCode.LayerName, layersNames[i]);
                        SelectionFilter filter = new SelectionFilter(value);
                        PromptSelectionResult PSR = editor.SelectAll(filter);
                        SelectionSet setOfLines = PSR.Value;

                        #region remove unrequired lines
                        List<Line> newSetOfLines = new List<Line>();
                        foreach (SelectedObject SO in setOfLines)
                        {
                            Line line = transaction.GetObject(SO.ObjectId, OpenMode.ForRead) as Line;
                            newSetOfLines.Add(line);
                        }

                        List<Point2d> midPointList = new List<Point2d>();
                        foreach (SelectedObject SO in setOfLines)
                        {
                            //get the midPoint  
                            Line line = transaction.GetObject(SO.ObjectId, OpenMode.ForRead) as Line;
                            double x1 = line.StartPoint.X; double y1 = line.StartPoint.Y;
                            double x2 = line.EndPoint.X; double y2 = line.EndPoint.Y;
                            Point2d midPoint = new Point2d((x1 + x2) / 2, (y1 + y2) / 2);
                            midPointList.Add(midPoint);
                        }
                        for (int j = 0; j < midPointList.Count; j++)
                        {
                            List<double> distList = new List<double>();
                            for (int k = 0; k < midPointList.Count; k++)
                            {
                                distList.Add(midPointList[j].GetDistanceTo(midPointList[k]));
                            }
                            distList.Remove(0);
                            int p2Index = distList.IndexOf(distList.Min()) + 1;

                            if (midPointList[p2Index].Y < midPointList[j].Y)
                            {
                                midPointList.RemoveAt(p2Index);
                                newSetOfLines.RemoveAt(p2Index);
                            }
                            else
                            {
                                midPointList.RemoveAt(p2Index);
                                newSetOfLines[j] = newSetOfLines[p2Index];
                                newSetOfLines.RemoveAt(p2Index);
                            }
                        }

                        #endregion

                        //open BlockTableRecord to add the generated tags
                        BlockTableRecord BTR = transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                        //loop for each line to set the tag 
                        foreach (Line line in newSetOfLines)
                        {
                            //get the startPoint and the endPoint 
                            double x1 = line.StartPoint.X;
                            double y1 = line.StartPoint.Y;
                            double x2 = line.EndPoint.X;
                            double y2 = line.EndPoint.Y;
                            if (x1 > x2)
                            {
                                x1 = x2;
                                x2 = line.StartPoint.X;
                                y1 = y2;
                                y2 = line.StartPoint.Y;

                            }

                            //construct the tag
                            MText tag = new MText();
                            tag.Layer = "Beam Tags";
                            tag.Contents = tagsText[i];

                            //set tag location and rotation
                            if (y1 == y2)
                            {
                                tag.Location = new Point3d(((x1 + x2) / 2) - e, y1 + constant, 0);

                            }
                            else if (x1 == x2)
                            {
                                tag.Location = new Point3d(x1 - constant, ((y1 + y2) / 2) - e, 0);
                                tag.Rotation = 90 * Math.PI / 180;
                            }
                            else if (y2 > y1)
                            {
                                double angle = Math.Atan2(y2 - y1, x2 - x1) - Math.Atan2(0.4, e);
                                double w = Math.Sqrt(Math.Pow(0.4, 2) + Math.Pow(e, 2));
                                tag.Location = new Point3d(((x1 + x2) / 2) - w * (Math.Cos(angle)), ((y1 + y2) / 2) - w * (Math.Sin(angle)), 0); ;
                                tag.Rotation = Math.Atan2(y2 - y1, x2 - x1);

                            }
                            else
                            {
                                double angle = Math.Abs(Math.Atan2(y2 - y1, x2 - x1)) + Math.Atan2(0.4, e);
                                double w = Math.Sqrt(Math.Pow(0.4, 2) + Math.Pow(e, 2));
                                tag.Location = new Point3d(((x1 + x2) / 2) - w * (Math.Cos(angle)), ((y1 + y2) / 2) + w * (Math.Sin(angle)), 0); ;
                                tag.Rotation = Math.Atan2(y2 - y1, x2 - x1);

                            }
                            //add the tag to the database
                            BTR.AppendEntity(tag);
                            transaction.AddNewlyCreatedDBObject(tag, true);

                        }
                    }




                    transaction.Commit();
                }
                catch (System.Exception ex)
                {
                    transaction.Abort();
                }



            }

        }
        #endregion

        #region New Column Dim
        public static void ColumnTag(string columnLayerName, string axesLayerName, string dimLayerName, string selectedUnit, string selectedDimStyle, bool flag)
        {
            #region handle the selected unit
            int adjust = 0;
            switch (selectedUnit)
            {
                case "m":
                    adjust = 1;
                    break;
                case "cm":
                    adjust = 100;
                    break;
                case "mm":
                    adjust = 1000;
                    break;
            }
            #endregion

            Document doc = Application.DocumentManager.MdiActiveDocument;
            DocumentLock dl = doc.LockDocument();
            Database db = doc.Database;
            Editor editor = doc.Editor;

            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                try
                {
                    #region handle the selected DimStyle
                    DimStyleTable DST = transaction.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                    foreach (ObjectId item in DST)
                    {
                        if ((transaction.GetObject(item, OpenMode.ForRead) as DimStyleTableRecord).Name == selectedDimStyle)
                        {
                            db.Dimstyle = item;
                        }
                    }
                    #endregion

                    #region Create Dim Layer

                    //creat new layer for dimensions if it's not existed
                    LayerTable layerTable = transaction.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    if (!(layerTable.Has(dimLayerName)))
                    {
                        LayerTableRecord dimLayer = new LayerTableRecord();
                        dimLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 3);
                        dimLayer.Name = dimLayerName;
                        layerTable.Add(dimLayer);
                        transaction.AddNewlyCreatedDBObject(dimLayer, true);
                    }
                    db.Clayer = layerTable[dimLayerName];
                    #endregion

                    #region Columns

                    //select all the lines at the column layer
                    TypedValue[] value1 = new TypedValue[2];
                    value1[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    value1[1] = new TypedValue((int)DxfCode.LayerName, columnLayerName);
                    SelectionFilter filter1 = new SelectionFilter(value1);
                    PromptSelectionResult PSR1 = editor.SelectAll(filter1);
                    SelectionSet setOfColumns = PSR1.Value;

                    List<Polyline> newSetOfCoulmns = new List<Polyline>();
                    //represent each col as a point
                    foreach (SelectedObject item in setOfColumns)
                    {
                        Polyline polyLine = transaction.GetObject(item.ObjectId, OpenMode.ForWrite) as Polyline;
                        newSetOfCoulmns.Add(polyLine);
                    }
                    #endregion

                    #region Axes

                    //select all the lines at the axes layer
                    TypedValue[] value2 = new TypedValue[2];
                    value2[0] = new TypedValue((int)DxfCode.Start, "Line");
                    value2[1] = new TypedValue((int)DxfCode.LayerName, axesLayerName);
                    SelectionFilter filter2 = new SelectionFilter(value2);
                    PromptSelectionResult PSR2 = editor.SelectAll(filter2);
                    SelectionSet setOfAxesLines = PSR2.Value;

                    //sperate the H_Lines from the V_Lines
                    List<Line> VLineOfaxes = new List<Line>();
                    List<Line> HLineOfaxes = new List<Line>();

                    for (int i = 0; i < setOfAxesLines.Count; i++)
                    {
                        Line line = transaction.GetObject(setOfAxesLines[i].ObjectId, OpenMode.ForWrite) as Line;
                        double x1 = Math.Round(line.StartPoint.X, 2);
                        double y1 = Math.Round(line.StartPoint.Y, 2);
                        double x2 = Math.Round(line.EndPoint.X, 2);
                        double y2 = Math.Round(line.EndPoint.Y, 2);
                        if (x1 == x2)
                        {
                            VLineOfaxes.Add(line);
                        }
                        else if (y1 == y2)
                        {
                            HLineOfaxes.Add(line);
                        }
                    }
                    #endregion

                    #region Points of axes intersection

                    List<Point2d> pointsList = new List<Point2d>();
                    for (int i = 0; i < VLineOfaxes.Count; i++)
                    {
                        for (int j = 0; j < HLineOfaxes.Count; j++)
                        {
                            pointsList.Add(new Point2d(VLineOfaxes[i].StartPoint.X, HLineOfaxes[j].StartPoint.Y));
                        }

                    }

                    #endregion

                    #region  nearest intersection Point for each column
                    //loop for each column
                    BlockTableRecord BTR = transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                    for (int i = 0; i < newSetOfCoulmns.Count; i++)
                    {
                        //get the four points of the column
                        List<Point2d> vertices = new List<Point2d>();
                        for (int j = 0; j < newSetOfCoulmns[i].NumberOfVertices; j++)
                        {
                            vertices.Add(newSetOfCoulmns[i].GetPoint2dAt(j));
                        }
                        // get the nearest intersection Point for the column
                        Point2d nearestIntersectionPoint = new Point2d();
                        List<double> distList = new List<double>();
                        foreach (Point2d item in pointsList)
                        {
                            distList.Add(item.GetDistanceTo(vertices[0]));
                            int nearestPointIndex = distList.IndexOf(distList.Min());
                            nearestIntersectionPoint = pointsList[nearestPointIndex];
                        }
                        // get the nearest vertix to the nearest intersection Point
                        distList.Clear();
                        Point2d nearestVertix = new Point2d();
                        Point2d preNearestVertix = new Point2d();
                        Point2d postNearestVertix = new Point2d();
                        foreach (Point2d item in vertices)
                        {
                            distList.Add(nearestIntersectionPoint.GetDistanceTo(item));
                            int nearestVertixIndex = distList.IndexOf(distList.Min());
                            nearestVertix = vertices[nearestVertixIndex];
                            preNearestVertix = nearestVertixIndex != 0 ? vertices[nearestVertixIndex - 1] : vertices[vertices.Count - 1];
                            postNearestVertix = nearestVertixIndex != vertices.Count - 1 ? vertices[nearestVertixIndex + 1] : vertices[0];
                        }
                        if (Math.Round(nearestVertix.X, 2) != Math.Round(nearestIntersectionPoint.X, 2) && Math.Round(nearestVertix.Y, 2) != Math.Round(nearestIntersectionPoint.Y, 2))
                        {
                            #region draw dimension in X_Direction
                            AlignedDimension dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            double dist = nearestIntersectionPoint.X - nearestVertix.X;
                            dim.XLine2Point = new Point3d(nearestVertix.X + dist, nearestVertix.Y, 0);
                            if (nearestVertix.Y > nearestIntersectionPoint.Y)
                            {
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y + 0.2 * adjust, 0);
                            }
                            else
                            {
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y - 0.2 * adjust, 0);
                            }
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion

                            #region draw dimension in Y_Direction
                            dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            dist = nearestIntersectionPoint.Y - nearestVertix.Y;
                            dim.XLine2Point = new Point3d(nearestVertix.X, nearestVertix.Y + dist, 0);
                            if (nearestVertix.X > nearestIntersectionPoint.X)
                            {
                                dim.DimLinePoint = new Point3d(nearestVertix.X + 0.2 * adjust, nearestVertix.Y, 0);
                            }
                            else
                            {
                                dim.DimLinePoint = new Point3d(nearestVertix.X - 0.2 * adjust, nearestVertix.Y, 0);
                            }
                            dim.TextRotation = 1.5708;
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion

                            // check if the column intersect with the grids to draw 2 extra dimensions
                            if (flag && ((preNearestVertix.Y > nearestIntersectionPoint.Y && postNearestVertix.Y < nearestIntersectionPoint.Y) || (preNearestVertix.Y < nearestIntersectionPoint.Y && postNearestVertix.Y > nearestIntersectionPoint.Y)))
                            {
                                if (preNearestVertix.X == nearestVertix.X)
                                {
                                    //draw Dim in X-Dir with the postNearestPoint
                                    #region draw dimension in X_Direction
                                    dim = new AlignedDimension();
                                    dim.XLine1Point = new Point3d(postNearestVertix.X, postNearestVertix.Y, 0);
                                    dist = nearestIntersectionPoint.X - postNearestVertix.X;
                                    dim.XLine2Point = new Point3d(postNearestVertix.X + dist, postNearestVertix.Y, 0);
                                    if (postNearestVertix.Y > nearestIntersectionPoint.Y)
                                    {
                                        dim.DimLinePoint = new Point3d(postNearestVertix.X, postNearestVertix.Y + 0.2 * adjust, 0);

                                    }
                                    else
                                    {
                                        dim.DimLinePoint = new Point3d(postNearestVertix.X, postNearestVertix.Y - 0.2 * adjust, 0);
                                    }
                                    dim.DimensionStyle = db.Dimstyle;
                                    BTR.AppendEntity(dim);
                                    transaction.AddNewlyCreatedDBObject(dim, true);
                                    #endregion
                                    //draw Dim in Y-Dir with the preNearestPoint
                                    #region draw dimension in Y_Direction
                                    dim = new AlignedDimension();
                                    dim.XLine1Point = new Point3d(preNearestVertix.X, preNearestVertix.Y, 0);
                                    dist = nearestIntersectionPoint.Y - preNearestVertix.Y;
                                    dim.XLine2Point = new Point3d(preNearestVertix.X, preNearestVertix.Y + dist, 0);
                                    if (preNearestVertix.X > nearestIntersectionPoint.X)
                                    {
                                        dim.DimLinePoint = new Point3d(preNearestVertix.X + 0.2 * adjust, preNearestVertix.Y, 0);
                                    }
                                    else
                                    {
                                        dim.DimLinePoint = new Point3d(preNearestVertix.X - 0.2 * adjust, preNearestVertix.Y, 0);
                                    }
                                    dim.TextRotation = 1.5708;
                                    dim.DimensionStyle = db.Dimstyle;
                                    BTR.AppendEntity(dim);
                                    transaction.AddNewlyCreatedDBObject(dim, true);
                                    #endregion
                                }
                                else
                                {
                                    //draw Dim in X-Dir with the preNearestPoint
                                    #region draw dimension in X_Direction
                                    dim = new AlignedDimension();
                                    dim.XLine1Point = new Point3d(preNearestVertix.X, preNearestVertix.Y, 0);
                                    dist = nearestIntersectionPoint.X - preNearestVertix.X;
                                    dim.XLine2Point = new Point3d(preNearestVertix.X + dist, preNearestVertix.Y, 0);
                                    if (preNearestVertix.Y > nearestIntersectionPoint.Y)
                                    {
                                        dim.DimLinePoint = new Point3d(preNearestVertix.X, preNearestVertix.Y + 0.2 * adjust, 0);
                                    }
                                    else
                                    {
                                        dim.DimLinePoint = new Point3d(preNearestVertix.X, preNearestVertix.Y - 0.2 * adjust, 0);
                                    }
                                    dim.DimensionStyle = db.Dimstyle;
                                    BTR.AppendEntity(dim);
                                    transaction.AddNewlyCreatedDBObject(dim, true);
                                    #endregion
                                    //draw Dim in Y-Dir with the postNearestPoint
                                    #region draw dimension in Y_Direction
                                    dim = new AlignedDimension();
                                    dim.XLine1Point = new Point3d(postNearestVertix.X, postNearestVertix.Y, 0);
                                    dist = nearestIntersectionPoint.Y - postNearestVertix.Y;
                                    dim.XLine2Point = new Point3d(postNearestVertix.X, postNearestVertix.Y + dist, 0);
                                    if (postNearestVertix.X > nearestIntersectionPoint.X)
                                    {
                                        dim.DimLinePoint = new Point3d(postNearestVertix.X + 0.2 * adjust, postNearestVertix.Y, 0);
                                    }
                                    else
                                    {
                                        dim.DimLinePoint = new Point3d(postNearestVertix.X - 0.2 * adjust, postNearestVertix.Y, 0);
                                    }
                                    dim.TextRotation = 1.5708;
                                    dim.DimensionStyle = db.Dimstyle;
                                    BTR.AppendEntity(dim);
                                    transaction.AddNewlyCreatedDBObject(dim, true);
                                    #endregion
                                }

                            }
                        }
                        else if (Math.Round(nearestVertix.X, 2) != Math.Round(nearestIntersectionPoint.X, 2))
                        {
                            #region draw dimension in X_Direction
                            AlignedDimension dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            double dist = nearestIntersectionPoint.X - nearestVertix.X;
                            dim.XLine2Point = new Point3d(nearestVertix.X + dist, nearestVertix.Y, 0);
                            if (nearestVertix.Y > nearestIntersectionPoint.Y)
                            {
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y + 0.2 * adjust, 0);
                            }
                            else
                            {
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y - 0.2 * adjust, 0);
                            }
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion
                        }
                        else if (Math.Round(nearestVertix.Y, 2) != Math.Round(nearestIntersectionPoint.Y, 2))
                        {
                            #region draw dimension in Y_Direction

                            AlignedDimension dim2 = new AlignedDimension();
                            dim2.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            double dist2 = nearestIntersectionPoint.Y - nearestVertix.Y;
                            dim2.XLine2Point = new Point3d(nearestVertix.X, nearestVertix.Y + dist2, 0);
                            if (nearestVertix.X > nearestIntersectionPoint.X)
                            {
                                dim2.DimLinePoint = new Point3d(nearestVertix.X + 0.2 * adjust, nearestVertix.Y, 0);
                            }
                            else
                            {
                                dim2.DimLinePoint = new Point3d(nearestVertix.X - 0.2 * adjust, nearestVertix.Y, 0);
                            }
                            dim2.TextRotation = 1.5708;
                            dim2.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim2);
                            transaction.AddNewlyCreatedDBObject(dim2, true);
                            #endregion


                        }

                        //draw the external dimension of the column
                        if (Math.Round(nearestVertix.Y, 2) == Math.Round(preNearestVertix.Y, 2))
                        {
                            //draw Dim in X-Dir with the preNearestPoint
                            #region draw dimension in X_Direction
                            AlignedDimension dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            dim.XLine2Point = new Point3d(preNearestVertix.X, preNearestVertix.Y, 0);
                            if (nearestVertix.Y > postNearestVertix.Y)
                            {
                                double dist = nearestVertix.Y - postNearestVertix.Y + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y - dist, 0);
                            }
                            else
                            {
                                double dist = postNearestVertix.Y - nearestVertix.Y + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y + dist, 0);
                            }
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion
                            //draw Dim in Y-Dir with the postNearestPoint
                            #region draw dimension in Y_Direction
                            dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            dim.XLine2Point = new Point3d(postNearestVertix.X, postNearestVertix.Y, 0);
                            if (nearestVertix.X > preNearestVertix.X)
                            {
                                double dist = nearestVertix.X - preNearestVertix.X + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(postNearestVertix.X - dist, postNearestVertix.Y, 0);
                            }
                            else
                            {
                                double dist = preNearestVertix.X - nearestVertix.X + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(postNearestVertix.X + dist, postNearestVertix.Y, 0);
                            }
                            dim.TextRotation = 1.5708;
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion
                        }
                        else
                        {
                            //draw Dim in X-Dir with the postNearestPoint
                            #region draw dimension in X_Direction
                            AlignedDimension dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            dim.XLine2Point = new Point3d(postNearestVertix.X, postNearestVertix.Y, 0);
                            if (nearestVertix.Y > preNearestVertix.Y)
                            {
                                double dist = nearestVertix.Y - preNearestVertix.Y + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y - dist, 0);

                            }
                            else
                            {
                                double dist = preNearestVertix.Y - nearestVertix.Y + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(nearestVertix.X, nearestVertix.Y + dist, 0);
                            }
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion
                            //draw Dim in Y-Dir with the preNearestPoint
                            #region draw dimension in Y_Direction
                            dim = new AlignedDimension();
                            dim.XLine1Point = new Point3d(nearestVertix.X, nearestVertix.Y, 0);
                            dim.XLine2Point = new Point3d(preNearestVertix.X, preNearestVertix.Y, 0);
                            if (nearestVertix.X > postNearestVertix.X)
                            {
                                double dist = nearestVertix.X - postNearestVertix.X + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(nearestVertix.X - dist, nearestVertix.Y, 0);
                            }
                            else
                            {
                                double dist = postNearestVertix.X - nearestVertix.X + 0.2 * adjust;
                                dim.DimLinePoint = new Point3d(nearestVertix.X + dist, nearestVertix.Y, 0);
                            }
                            dim.TextRotation = 1.5708;
                            dim.DimensionStyle = db.Dimstyle;
                            BTR.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                            #endregion
                        }

                    }

                    #endregion

                    transaction.Commit();
                }
                catch (System.Exception)
                {

                    transaction.Abort();
                }
            }
        }
        #endregion

        #region axesDim
        public static void AxesDimension(string axesLayerName,string selectedUnit,string selectedDimStyle,string dimLayerName)
        {
            double movement = 1;
            #region handle the selected unit
            int adjust = 1;
            switch (selectedUnit)
            {
                case "m":
                    adjust = 1;
                    break;
                case "cm":
                    adjust = 100;
                    break;
                case "mm":
                    adjust = 1000;
                    break;
            }
            #endregion

            Document doc = Application.DocumentManager.MdiActiveDocument;
            DocumentLock dl = doc.LockDocument();
            Database db = doc.Database;
            Editor editor = doc.Editor;
            DocumentLock m = doc.LockDocument();
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                #region handle the selected DimStyle
                DimStyleTable DST = transaction.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                foreach (ObjectId item in DST)
                {
                    if ((transaction.GetObject(item, OpenMode.ForRead) as DimStyleTableRecord).Name == selectedDimStyle)
                    {
                        db.Dimstyle = item;
                    }
                }
                #endregion

                #region Create Dim Layer

                //creat new layer for dimensions if it's not existed
                LayerTable layerTable = transaction.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                if (!(layerTable.Has(dimLayerName)))
                {
                    LayerTableRecord dimLayer = new LayerTableRecord();
                    dimLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 3);
                    dimLayer.Name = dimLayerName;
                    layerTable.Add(dimLayer);
                    transaction.AddNewlyCreatedDBObject(dimLayer, true);
                }
                db.Clayer = layerTable[dimLayerName];
                #endregion

                BlockTableRecord BTR = transaction.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                #region Get V and H Axes

                //select all the lines at the axes layer
                TypedValue[] value2 = new TypedValue[2];
                value2[0] = new TypedValue((int)DxfCode.Start, "Line");
                value2[1] = new TypedValue((int)DxfCode.LayerName, axesLayerName);
                SelectionFilter filter2 = new SelectionFilter(value2);
                PromptSelectionResult PSR2 = editor.GetSelection(filter2);
                SelectionSet setOfAxesLines = PSR2.Value;

                //sperate the H_Lines from the V_Lines
                List<Line> VLineOfaxes = new List<Line>();
                List<Line> HLineOfaxes = new List<Line>();

                for (int i = 0; i < setOfAxesLines.Count; i++)
                {
                    Line line = transaction.GetObject(setOfAxesLines[i].ObjectId, OpenMode.ForWrite) as Line;
                    double startPoint_x = Math.Round(line.StartPoint.X, 2);
                    double startPoint_y = Math.Round(line.StartPoint.Y, 2);
                    double endPoint_x = Math.Round(line.EndPoint.X, 2);
                    double endPoint_y = Math.Round(line.EndPoint.Y, 2);
                    if (startPoint_x == endPoint_x)
                    {
                        VLineOfaxes.Add(line);
                    }
                    else if (startPoint_y == endPoint_y)
                    {
                        HLineOfaxes.Add(line);
                    }
                }

                VLineOfaxes.Sort(delegate (Line l1, Line l2)
                                    {
                                        return l1.StartPoint.X.CompareTo(l2.StartPoint.X);
                                    });
                HLineOfaxes.Sort(delegate (Line l1, Line l2)
                                    {
                                        return l1.StartPoint.Y.CompareTo(l2.StartPoint.Y);
                                    });
                #endregion

                #region Draw dimension in X-Direction
                AlignedDimension dim;
                double x1 = 0, y1 = 0, x2, y2;
                for (int i = 0; i < VLineOfaxes.Count - 1; i++)
                {
                    x1 = VLineOfaxes[i].StartPoint.X;
                    y1 = VLineOfaxes[i].StartPoint.Y > VLineOfaxes[i].EndPoint.Y ? VLineOfaxes[i].StartPoint.Y : VLineOfaxes[i].EndPoint.Y;
                    x2 = VLineOfaxes[i + 1].StartPoint.X;
                    dim = new AlignedDimension()
                    {
                        XLine1Point = new Point3d(x1, y1 - movement * adjust, 0),
                        XLine2Point = new Point3d(x2, y1 - movement * adjust, 0),
                        DimLinePoint = new Point3d(x1, y1 - movement * adjust, 0),
                        DimensionStyle = db.Dimstyle,
                    };
                    BTR.AppendEntity(dim);
                    transaction.AddNewlyCreatedDBObject(dim, true);
                }
                // draw the overall dim
                dim = new AlignedDimension()
                {
                    XLine1Point = new Point3d(VLineOfaxes[0].StartPoint.X, y1 - (movement - .5) * adjust, 0),
                    XLine2Point = new Point3d(VLineOfaxes[VLineOfaxes.Count - 1].StartPoint.X, y1 - (movement - .5) * adjust, 0),
                    DimLinePoint = new Point3d(VLineOfaxes[0].StartPoint.X, y1 - (movement - .5) * adjust, 0),
                    DimensionStyle = db.Dimstyle

                };
                BTR.AppendEntity(dim);
                transaction.AddNewlyCreatedDBObject(dim, true);
                #endregion

                #region Draw dimension in Y-Direction
                for (int i = 0; i < HLineOfaxes.Count - 1; i++)
                {
                    x1 = HLineOfaxes[i].StartPoint.X < HLineOfaxes[i].EndPoint.X ? HLineOfaxes[i].StartPoint.X : HLineOfaxes[i].EndPoint.X;
                    y1 = HLineOfaxes[i].StartPoint.Y;
                    y2 = HLineOfaxes[i + 1].StartPoint.Y;
                    dim = new AlignedDimension()
                    {
                        XLine1Point = new Point3d(x1 + movement * adjust, y1, 0),
                        XLine2Point = new Point3d(x1 + movement * adjust, y2, 0),
                        DimLinePoint = new Point3d(x1 + movement * adjust, y1, 0),
                        DimensionStyle = db.Dimstyle,
                        TextRotation = 1.5708
                    };
                    BTR.AppendEntity(dim);
                    transaction.AddNewlyCreatedDBObject(dim, true);
                }
                // draw the overall dim
                dim = new AlignedDimension()
                {
                    XLine1Point = new Point3d(x1 + (movement - .5) * adjust, HLineOfaxes[0].StartPoint.Y, 0),
                    XLine2Point = new Point3d(x1 + (movement - .5) * adjust, HLineOfaxes[HLineOfaxes.Count - 1].StartPoint.Y, 0),
                    DimLinePoint = new Point3d(x1 + (movement - .5) * adjust, HLineOfaxes[0].StartPoint.Y, 0),
                    DimensionStyle = db.Dimstyle,
                    TextRotation = 1.5708
                };
                BTR.AppendEntity(dim);
                transaction.AddNewlyCreatedDBObject(dim, true);
                #endregion

                transaction.Commit();
            }
        }
        #endregion

        //public static void AxesDimension(string axesLayerName = "axes", string dimLayerName = "axes Dim", string selectedUnit = "m")


        [CommandMethod("BeamTag")]
        public void BeamTag()
        {
            BeamTag m = new BeamTag();
            Application.ShowModalWindow(m);
        }

        [CommandMethod("ColDim")]
        public void ColDim()
        {
            ColumnDim m = new ColumnDim();
            Application.ShowModalWindow(m);
        }

        [CommandMethod("axesDim")]
        public void AxesDim()
        {
            AxesDim m = new AxesDim();
            Application.ShowModalWindow(m);
        }



    }
}

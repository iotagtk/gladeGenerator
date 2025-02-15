﻿using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace GladeGeneratorGUI
{
    partial class MainWindow
    {

        /// <summary>
        /// ToplevelとChildIdのKeyを生成する
        /// </summary>
        /// <returns></returns>
        private string _getTopLevelPartChildPartKey(ChildLevelPart ChildLevelPart1)
        {
            if (
                SelectedGladeDataRow == null || 
                SelectedTopLevelPartRow == null ||
                ChildLevelPart1 == null ||
                SelectedGladeDataRow.OutPutGladeName == null || 
                ((TopLevelPart)SelectedTopLevelPartRow).PartId == "" 
                || ChildLevelPart1.PartId == ""
                )
            {
                Console.WriteLine(" 例外 TopLevelPartChildPartKeyの元データがない ");
                return "";
            }

            if (clsArgsConfig.Instance().ProjectName == null){
                
                Console.WriteLine(" 例外 ProjectNameがない ");
                return "";
            }
            
           string GladeName = ((GladeData)SelectedGladeDataRow).OutPutGladeName; 
           string topLevelPartKey = ((TopLevelPart)SelectedTopLevelPartRow).PartId;
           string childLevelPartKey = ((ChildLevelPart)ChildLevelPart1).PartId;

           string GladeNametopLevelPartKeychildLevelPartKey = clsArgsConfig.Instance().ProjectName + GladeName + topLevelPartKey + childLevelPartKey;

           return GladeNametopLevelPartKeychildLevelPartKey._md5();
        }
        
        /// <summary>
        /// Toplevelを生成する
        /// </summary>
        /// <returns></returns>
        private string _getTopLevelPartKey()
        {
            if ( 
                SelectedGladeDataRow == null || 
                SelectedTopLevelPartRow == null ||
                SelectedGladeDataRow.OutPutGladeName == null ||
                ((TopLevelPart)SelectedTopLevelPartRow).PartId == "" )
            {
                Console.WriteLine(" 例外 TopLevelPartKeyの元データがない ");
                return "";
            }

            if (clsArgsConfig.Instance().ProjectName == null){
                
                Console.WriteLine(" 例外 ProjectNameがない ");
                return "";
            }

            string GladeName = ((GladeData)SelectedGladeDataRow).OutPutGladeName; 
            string topLevelPartKey = ((TopLevelPart)SelectedTopLevelPartRow).PartId;

            var GladeNameTopLevelPartKey = clsArgsConfig.Instance().ProjectName + GladeName + topLevelPartKey;

            return GladeNameTopLevelPartKey._md5() ;
        }

        private string _getGradeKey()
        {
            if (
                SelectedGladeDataRow == null || 
                SelectedGladeDataRow.OutPutGladeName == null
               )
            {
                Console.WriteLine(" 例外 GradeKeyがない ");
                return "";
            }
            
            if (clsArgsConfig.Instance().ProjectName == null){
                
                Console.WriteLine(" 例外 ProjectNameがない ");
                return "";
            }

            var GladeNameTopLevelPartKey = clsArgsConfig.Instance().ProjectName + ((GladeData)SelectedGladeDataRow).OutPutGladeName; 

            return GladeNameTopLevelPartKey._md5() ;
        }




    }
}
/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 26-Jun-16
 * Time: 11:00 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FeatherMvvm.Attributes;
using DashMvvm.Attributes;

namespace FeatherMvvm.Binding.Components
{
	/// <summary>
	/// Description of ListViewHelper.
	/// </summary>
	internal class ListViewHelper
	{
		public ListViewHelper()
		{
		}
		
        private string GetGroupingColumnName(object item)
        {
            foreach (var property in item.GetType().GetProperties())
            {
                if(property.GetCustomAttribute<ListViewGroupingColumn>() != null)
                {
                    var result = property.GetValue(item);
                    if(result == null)
                    {
                        return string.Empty;
                    }
                    return result.ToString();
                }
            }
            return string.Empty;
        }
		public void PopulateList(ListView lv,object viewModel,PropertyInfo viewModelProperty)
		{
			if(lv.InvokeRequired)
			{
				lv.Invoke((MethodInvoker)delegate
					{
						PopulateList(lv, viewModel, viewModelProperty);
					});
				return;
			}
			
			IEnumerable list = viewModelProperty.GetValue(viewModel) as IEnumerable;
		
			lv.Items.Clear();
			if(list == null)
			{
				return;
			}

			var listedProps = GetColumnHeaderProperties(viewModel, viewModelProperty);
			if (listedProps != null && listedProps.Count > 0)
			{
                bool gottenGrouping = false;
                string groupingColumn = string.Empty;
				foreach (var item in list)
				{
                    if(!gottenGrouping)
                    {
                        groupingColumn = GetGroupingColumnName(item);
                        gottenGrouping = true;
                    }
				    var cellValue = listedProps[0].GetValue(item);
				    if (cellValue == null)
				    {
				        cellValue = "";
				    }

					ListViewItem lvi = lv.Items.Add(cellValue.ToString());
					for (int i = 1; i < listedProps.Count; i++)
					{
					    var subItem = listedProps[i].GetValue(item) ?? String.Empty;

                        lvi.SubItems.Add(subItem.ToString());
					}
					lvi.Tag = item;
                    AddToGroup(lv, lvi, groupingColumn);
				}
			}
			else
			{
				foreach (var item in list)
				{
					
					ListViewItem lvi = lv.Items.Add(item.ToString());
					lvi.Tag = item;

				}
			}
		}
		

        private void AddToGroup(ListView listView, ListViewItem lvi,string groupingColumn)
        {
            
            if(string.IsNullOrEmpty(groupingColumn))
            {
                return;
            }


            var group = lvi.Tag.GetType().GetProperty(groupingColumn).GetValue(lvi.Tag);
            if(group == null)
            {
                return;
            }
            string groupName = group.ToString();
            if(string.IsNullOrEmpty(groupName))
            {
                return;
            }
            var foundIt = false;
            var headerIndex = 0;
            for (int i = 0; i < listView.Groups.Count; i++)
            {
                if(listView.Groups[i].Header == groupName)
                {
                    foundIt = true;
                    headerIndex = i;
                    break;
                }
            }
            if(!foundIt)
            {
                listView.Groups.Add(new ListViewGroup(groupName,HorizontalAlignment.Left));
                headerIndex = listView.Groups.Count - 1;
            }
            lvi.Group = listView.Groups[headerIndex];

        }

		private List<PropertyInfo> GetColumnHeaderProperties(object viewModel, PropertyInfo viewModelProperty)
		{
			if(viewModelProperty.PropertyType.GenericTypeArguments.Length == 0)
			{
				throw new Exception("Only lists are currently supported for supplying ListView columns");
			}
		    var type = viewModelProperty.PropertyType.GenericTypeArguments[0];
		    if (type.IsInterface)
		    {
		        return null;
		    }
            Type listType = InferListTypeFromContent(viewModel, viewModelProperty) ??
                            InferListTypeFromViewModelProperty(viewModelProperty);

            var columnSource = Activator.CreateInstance(listType);
			return columnSource.GetType()
				.GetProperties().Where(a => a.GetCustomAttribute<ListViewColumnAttribute>() != null && a.CanRead)
				.ToList();
		}

	    private Type InferListTypeFromContent(object viewModel, PropertyInfo viewModelProperty)
	    {
	        IList content = viewModelProperty.GetValue(viewModel) as IList;
	        if (content?.Count == 0)
	        {
	            return null;
	        }
	        var item = content?[0];
            return item?.GetType();
	    }

	    private Type InferListTypeFromViewModelProperty(PropertyInfo viewModelProperty)
	    {
	        return viewModelProperty.PropertyType.GenericTypeArguments[0];
	    }
		
		public void AddListViewColumns(ListView lv,object viewModel, PropertyInfo viewModelProperty)
		{
			if(lv.InvokeRequired)
			{
				lv.Invoke((MethodInvoker)delegate
					{
						AddListViewColumns(lv,viewModel, viewModelProperty);
					});
				return;
			}
			lv.Columns.Clear();
			lv.View = View.Details;
			List<float> widthWeights = new List<float>();
			if(viewModelProperty.PropertyType.GenericTypeArguments.Length == 0)
			{
				throw new Exception("ListView columns are currently only generated from lists");
			}

		    Type listType = InferListTypeFromContent(viewModel, viewModelProperty) ??
		                    InferListTypeFromViewModelProperty(viewModelProperty);

			var columnSource = Activator.CreateInstance(listType);
			foreach(PropertyInfo propInfo in columnSource.GetType().GetProperties())
			{
				if(propInfo.CanRead == false)
				{
					continue;
				}
				ListViewColumnAttribute colAttrib = propInfo.GetCustomAttribute<ListViewColumnAttribute>(false);
				if(colAttrib != null)
				{
					lv.Columns.Add(colAttrib.Title);					
					widthWeights.Add(colAttrib.WidthWeight);
				}
			}
			
			Action sizeColumns = () =>
			{
				float totalWeight = widthWeights.Sum();
				for(int i = 0; i < lv.Columns.Count; i++)
				{
					float columnWidthFraction = widthWeights[i] / totalWeight; 
					var width = lv.Width * columnWidthFraction;
					lv.Columns[i].Width = (int)Math.Ceiling(width);
				}
			};
			
			sizeColumns();
			lv.Resize += (sender, e) => sizeColumns();
		}
	}
}

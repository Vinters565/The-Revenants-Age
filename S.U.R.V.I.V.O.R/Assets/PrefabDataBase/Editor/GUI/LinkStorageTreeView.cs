using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DataBase
{
    public class LinkStorageTreeView : TreeView
    {
        private class EntryTreeViewItem : TreeViewItem
        {
            public Entry Entry { get; }

            public EntryTreeViewItem([NotNull] Entry entry)
            {
                Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            }
        }

        private enum ColumnId
        {
            Number,
            Address,
            Path,
        }

        private class SortParameters
        {
            public ColumnId sortId;
            public bool isDescending;
        }

        private readonly EditorPrefabDataBaseController controller;
        private SortParameters sortParameters;
        private TreeViewItem root;

        public LinkStorageTreeView(
            TreeViewState state,
            EditorPrefabDataBaseController controller)
            : this(state, controller, new MultiColumnHeader(CreateMultiColumnHeaderState()))
        {
        }

        private LinkStorageTreeView(
            TreeViewState state,
            EditorPrefabDataBaseController controller,
            MultiColumnHeader header) : base(state, header)
        {
            this.controller = controller;
            showBorder = true;
            header.sortingChanged += OnSortingChanged;
        }

        private void OnSortingChanged(MultiColumnHeader multicolumnheader)
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
            {
                sortParameters = null;
                return;
            }

            sortParameters = new SortParameters()
            {
                isDescending = !multiColumnHeader.IsSortedAscending(sortedColumns[0]),
                sortId = (ColumnId) sortedColumns[0]
            };
            Reload();
        }

        private Func<Entry, IComparable> GetOrderFunc(ColumnId columnId)
        {
            return columnId switch
            {
                ColumnId.Address => (x) => x.Address,
                ColumnId.Path => (x) => x.ResourcesPath,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override TreeViewItem BuildRoot()
        {
            root = new TreeViewItem {id = -1, depth = -1};
            var entries = controller.Storage.ToList();

            if (sortParameters != null)
            {
                entries = (sortParameters.isDescending
                        ? entries.OrderByDescending(GetOrderFunc(sortParameters.sortId))
                        : entries.OrderBy(GetOrderFunc(sortParameters.sortId)))
                    .ToList();
            }

            foreach (var (entry, i) in entries.Select((entry, i) => (entry, i)))
            {
                root.AddChild(new EntryTreeViewItem(entry) {id = i, depth = 0});
            }

            return root;
        }


        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            if (selectedIds.Count == 0)
                return;
            Selection.activeObject = FindItemInVisibleRows(selectedIds.Last()).Entry.Asset;
        }

        EntryTreeViewItem FindItemInVisibleRows(int id)
        {
            return GetRows()
                .Where(x => x.id == id)
                .Select(x => x as EntryTreeViewItem)
                .FirstOrDefault();
        }


        private GUIStyle labelStyle;

        protected override void RowGUI(RowGUIArgs args)
        {
            labelStyle ??= new GUIStyle("PR Label");
            var item = args.item as EntryTreeViewItem;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        private void CellGUI(Rect cellRect, EntryTreeViewItem item, int column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch ((ColumnId) column)
            {
                case ColumnId.Number:
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        labelStyle.Draw(cellRect, (item.id + 1).ToString(), false, false, args.selected, args.focused); 
                    }
                }
                    break;
                case ColumnId.Address:
                {
                    if (Event.current.type == EventType.Repaint)
                        labelStyle.Draw(cellRect, item.Entry.Address, false, false, args.selected, args.focused);
                }
                    break;
                case ColumnId.Path:
                    if (Event.current.type == EventType.Repaint)
                    {
                        var path = item.Entry.ResourcesPath;
                        if (string.IsNullOrEmpty(path))
                            path = "";
                        labelStyle.Draw(cellRect, path, false, false, args.selected, args.focused);
                    }

                    break;
            }
        }

        private static MultiColumnHeaderState CreateMultiColumnHeaderState()
        {
            return new MultiColumnHeaderState(GetColumns());

            MultiColumnHeaderState.Column[] GetColumns()
            {
                return new[]
                {
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent("Number"),
                        minWidth = 50,
                        width = 60,
                        maxWidth = 100,
                        headerTextAlignment = TextAlignment.Left,
                        autoResize = true,
                        canSort = false
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent("Address", "Address used to load asset at runtime"),
                        minWidth = 100,
                        width = 260,
                        maxWidth = 10000,
                        headerTextAlignment = TextAlignment.Left,
                        autoResize = true,
                        canSort = true
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent("Path", "Current Path of asset"),
                        minWidth = 100,
                        width = 300,
                        maxWidth = 10000,
                        headerTextAlignment = TextAlignment.Left,
                        autoResize = true,
                        canSort = true
                    },
                };
            }
        }
    }
}
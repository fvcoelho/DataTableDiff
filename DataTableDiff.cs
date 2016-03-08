namespace nutnet.Helpers
{
    public class DataTableDiff
    {
        private string _diffColumnName;

        public string DiffColumnName
        {
            get
            {
                if (string.IsNullOrEmpty(_diffColumnName))
                    _diffColumnName = "DiffDescriptio";

                return _diffColumnName;
            }
            set
            {
                _diffColumnName = value;
            }
        }
        public string[] IgnoreColumnName { get; set; }
        public bool HasEmptyRows { get; set; }

        private readonly DataTable _dt;

        public DataTableDiff(DataTable dt, string diffColumnName)
        {
            _dt = dt;
            _diffColumnName = diffColumnName;

            _dt.Columns.Add(DiffColumnName);
        }

        public DataTable GetColumnDiff()
        {
            var countRows = _dt.Rows.Count;
            for (var i = 0; i < countRows; i++)
                ForEachRow(i, countRows);

            if (!HasEmptyRows)
                RemoveEmptyRow();

            return _dt;
        }

        private void ForEachRow(int i, int countRows)
        {
            var isLastRow = (i + 1 == countRows);
            if (!isLastRow)
            {
                var firstRow = _dt.Rows[i];
                var secoundRow = _dt.Rows[i + 1];

                CompareEachColumns(firstRow, secoundRow, _dt, i);
            }
        }

        private void RemoveEmptyRow()
        {
            var countRows = _dt.Rows.Count;
            var currentRow = 0;
            foreach (DataRow row in _dt.Rows)
            {
                currentRow++;

                var isLastRow = (currentRow == countRows);
                if (!isLastRow)
                    if (string.IsNullOrEmpty(row[DiffColumnName].ToString()))
                        row.Delete();
            }
        }

        private void CompareEachColumns(DataRow firstRow, DataRow secoundRow, DataTable dt, int index)
        {
            var rowDesc = string.Empty;
            var indexDesc = dt.Columns[DiffColumnName].Ordinal;
            var countColumns = dt.Columns.Count;

            for (var cIndex = 0; cIndex < countColumns; cIndex++)
                rowDesc += CompareValues(dt.Columns, firstRow, secoundRow, cIndex);

            dt.Rows[index][indexDesc] = rowDesc.TrimEnd(new char[] { ',', ' ' });
        }

        private string CompareValues(DataColumnCollection dcc, DataRow firstRow, DataRow secoundRow, int cIndex)
        {
            var columnName = dcc[cIndex].ColumnName;
            var oldValue = firstRow[cIndex].ToString();
            var newValue = secoundRow[cIndex].ToString();

            return ConcatText(columnName, oldValue, newValue);
        }

        private string ConcatText(string columnName, string oldValue, string newValue)
        {
            var r = string.Empty;

            if (!IgnoreColumnName.Any(p => p.Contains(columnName)))
                if (oldValue != newValue)
                    r = string.Format("{0} de '{1}' para '{2}', ", columnName, newValue, oldValue);

            return r;
        }
    }
}
import React from 'react';
import { Table as BootstrapTable } from 'reactstrap';
import './tables.css';
import { ColumnElement, RowElement } from './model';

// interface TableBaseProps<T> {
//   columns: ReactDataGrid.Column<{ id: number; title: string }>[] | undefined;
//   rowGetter: { id: number; title: string }[] | ((rowIdx: number) => { id: number; title: string });
// }

export interface TableBaseProps {
  columns: ColumnElement[];
  rowGetter: RowElement[] | ((rowIdx: number) => RowElement) | any;
  rowCount: number;
}

export class Table extends React.PureComponent<TableBaseProps> {

  numColumns(): number {
    return this.props.columns.length;
  }

  renderHead(): JSX.Element {
    return (
      <thead>
        <tr>
          {this.props.columns.map(c => <th key={c.key}>{c.name}</th>)}
        </tr>
      </thead>)
  }

  renderBody(numRows: number): JSX.Element {
    const rows = [];
    for (let n = 0; n < numRows; n++) {
      rows.push(this.renderRow(n));
    }
    return (
      <tbody>
        {rows}
      </tbody>)
  }

  renderRow(n: number) {
    const row = Array.isArray(this.props.rowGetter) ? this.props.rowGetter[n] : this.props.rowGetter(n);
    const cells: JSX.Element[] = [];
    this.props.columns.forEach(c => {
      cells.push(<td key={c.key}>{row[c.key]}</td>)
    })
    return (
      <tr key={n}>
        {cells}
      </tr>)
  }

  render() {
    return (
      <div className="table-container">
        <BootstrapTable striped>
          {this.renderHead()}
          {this.renderBody(this.props.rowCount)}
        </BootstrapTable>
      </div>);
  }
}
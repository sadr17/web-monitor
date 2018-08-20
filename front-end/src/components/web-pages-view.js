import * as React from 'react';
import Paper from '@material-ui/core/Paper';
import {EditingState} from '@devexpress/dx-react-grid';
import {
  Grid,
  Table, 
  VirtualTable,
  TableHeaderRow,
  TableEditRow,
  TableEditColumn,
} from '@devexpress/dx-react-grid-material-ui';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import Button from '@material-ui/core/Button';
import IconButton from '@material-ui/core/IconButton';

import AddIcon from '@material-ui/icons/Add';
import DeleteIcon from '@material-ui/icons/Delete';
import EditIcon from '@material-ui/icons/Edit';
import SaveIcon from '@material-ui/icons/Save';
import CancelIcon from '@material-ui/icons/Cancel';
import * as signalR from '@aspnet/signalr'

const URL = 'http://localhost:54131/api/webpages';
  
  const AddButton = ({ onExecute }) => (
    <IconButton color='primary' onClick={onExecute} title="Create new record">
      <AddIcon />
    </IconButton>
  );
  
  const EditButton = ({ onExecute }) => (
    <IconButton onClick={onExecute} title="Edit record">
      <EditIcon />
    </IconButton>
  );
  
  const DeleteButton = ({ onExecute }) => (
    <IconButton onClick={onExecute} title="Delete record">
      <DeleteIcon />
    </IconButton>
  );
  
  const CommitButton = ({ onExecute }) => (
    <IconButton onClick={onExecute} title="Save changes">
      <SaveIcon />
    </IconButton>
  );
  
  const CancelButton = ({ onExecute }) => (
    <IconButton color="secondary" onClick={onExecute} title="Cancel changes">
      <CancelIcon />
    </IconButton>
  );
  
  const commandComponents = {
    add: AddButton,
    edit: EditButton,
    delete: DeleteButton,
    commit: CommitButton,
    cancel: CancelButton,
  };
  
  const Command = ({ id, onExecute }) => {
    const CommandButton = commandComponents[id];
    return (<CommandButton onExecute={onExecute} />);
  };
    
  const Cell = (props) => {
    return <Table.Cell {...props} />;
  };
  
  const EditCell = (props) => {
    return <TableEditRow.Cell {...props} />;
  };

  const sendRequest = (url, methodName, body) => {
    return fetch(url, {
      method: methodName,
      headers: {'Content-Type':'application/json', Accept: 'application/json'},
      body: body,
    }).catch(() => console.log('Error during request to server: method='+methodName+'.'));
  }

  const getRowId = (row) => row.id;

export class WebPagesView extends React.PureComponent {
  
  constructor(props) {
    super(props);

    this.state = {
      columns: [
        { name: 'id', title: 'Id #', width: 20 },
        { name: 'displayName', title: 'Name' },
        { name: 'link', title: 'Link' },
        { name: 'status', title: 'Status' },
      ],
      tableColumnExtensions: [
        { columnName: 'id', align: 'right' },
      ],
      editingStateColumnExtensions: [
        { columnName: 'id', editingEnabled: false },
        { columnName: 'status', editingEnabled: false },
      ],
      rows: [],
      editingRowIds: [],
      addedRows: [],
      rowChanges: {},
      deletingRows: [],
      loading: true,
      hubConnection: null,
    };
    this.getStateDeletingRows = this.getStateDeletingRows.bind(this);
    this.getStateRows = this.getStateRows.bind(this);
    this.changeEditingRowIds = this.changeEditingRowIds.bind(this);
    this.changeAddedRows = this.changeAddedRows.bind(this);
    this.changeRowChanges = this.changeRowChanges.bind(this);
    this.commitChanges = this.commitChanges.bind(this);
    this.cancelDelete = this.cancelDelete.bind(this);
    this.deleteRows = this.deleteRows.bind(this);
  }

  componentDidMount() {
    this.loadData();
    this.createHub()  
  }

  componentDidUpdate() {
    this.loadData();
  }

  getStateDeletingRows() {
    const { deletingRows } = this.state;
    return deletingRows;
  }

  getStateRows() {
    const { rows } = this.state;
    return rows;
  };

  changeEditingRowIds(editingRowIds) {
      this.setState({ editingRowIds });
  }

  changeAddedRows (addedRows) { 
    this.setState({
        addedRows: addedRows.map(row => (Object.keys(row).length ? row : {
        displayName: '',
        link: 'http://'
      })),
    });
  }

  changeRowChanges(rowChanges) { 
    this.setState({ rowChanges });
  }

  commitChanges ({ added, changed, deleted }) {
    let { rows } = this.state;
    if (added) {
      const addedPage = JSON.stringify(added[0]);
      sendRequest(URL, 'post', addedPage)
      .then(result => result.json())
      .then((data) => {
        rows = [...rows, data];
        this.setState({ rows });
      });        
    }
    else if (changed) {     
      const item = rows.filter(row => changed[row.id])[0];       
      const updatedPage = JSON.stringify({ ...item, ...changed[item.id] });
      sendRequest(URL, 'post', updatedPage)
      .then(result => result.json())
      .then((data) => {
        rows = rows.map(row => (changed[row.id] ? { ...row, ...data } : row));
        this.setState({ rows });
      });
    }
    else 
      this.setState({ deletingRows: deleted || this.getStateDeletingRows() });        
  }

  cancelDelete() {
    this.setState({ deletingRows: [] });
  }

  deleteRows() {
    const rows = this.getStateRows().slice();
    this.getStateDeletingRows().forEach((rowId) => {
      const index = rows.findIndex((row) => row.id === rowId);
      if (index > -1) {
        sendRequest(URL+ '/' + rowId, 'delete', null)
        .then((result) => {
          rows.splice(index, 1);
          this.setState({ rows, deletingRows: [] });
        });      
      }
    });      
  }

  createHub() {
    if (this.state.hubConnection !== null)
      return;

    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:54131/updateHub')
        .build();
    hubConnection.start()
        .catch(err => console.log('Error while establishing connection :('));
    hubConnection.on("UpdateStatus", (pages) => {
        this.setState({ rows: pages });
      });

    this.setState({ hubConnection });
  }

  loadData() {
    if (URL === this.lastQuery) {
      this.setState({ loading: false });
      return;
    }
    sendRequest(URL, 'GET', null)
    .then(result => result.json())
    .then((data) => {
        this.setState({
             rows: data,
             loading: false,
        });
    });
    this.lastQuery = URL;
  }

  render() {
    const {
      rows,
      columns,
      tableColumnExtensions,
      loading,
      editingRowIds,
      addedRows,
      rowChanges,
      deletingRows,
      editingStateColumnExtensions,
    } = this.state;

    return (
      <Paper style={{ position: 'relative' }}>
        <Grid rows={rows}
          columns={columns}
          getRowId={getRowId} >
          <EditingState  editingRowIds={editingRowIds}
            columnExtensions={editingStateColumnExtensions}
            onEditingRowIdsChange={this.changeEditingRowIds}
            rowChanges={rowChanges}
            onRowChangesChange={this.changeRowChanges}
            addedRows={addedRows}
            onAddedRowsChange={this.changeAddedRows}
            onCommitChanges={this.commitChanges} />
           <VirtualTable columnExtensions={tableColumnExtensions} />
          <TableHeaderRow />
          <TableEditRow cellComponent={EditCell} />
          <TableEditColumn width={120}
            showAddCommand={!addedRows.length}
            showEditCommand
            showDeleteCommand
            commandComponent={Command} />
        </Grid>
        <Dialog open={!!deletingRows.length}
          onClose={this.cancelDelete} >
          <DialogTitle>Delete record</DialogTitle>
          <DialogContent>
            <DialogContentText>Are you sure to delete the following record?</DialogContentText>
            <Paper>
              <Grid rows={rows.filter((row) => row.id === deletingRows[0])}
                columns={columns} >
                <Table columnExtensions={tableColumnExtensions} cellComponent={Cell} />
                <TableHeaderRow />
              </Grid>
            </Paper>
          </DialogContent>
          <DialogActions>
            <Button onClick={this.cancelDelete} color="primary">Cancel</Button>
            <Button onClick={this.deleteRows} color="secondary">Delete</Button>
          </DialogActions>
        </Dialog>
        {loading && <div>loading...</div>}
      </Paper>);
  }
}
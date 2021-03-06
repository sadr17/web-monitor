import * as React from 'react';
import Paper from '@material-ui/core/Paper';
import {EditingState, DataTypeProvider} from '@devexpress/dx-react-grid';
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
import Chip from '@material-ui/core/Chip';
import * as signalR from '@aspnet/signalr'

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

  const StatusFormatter = ({ value }) => {
    let color = value === 'OK' ? 'primary' : 'secondary';
    let text = value;
    if (value === null) {
      color = 'default';
      text = 'InProgress';
    }

    return (
       <Chip label={text} color={color} />
    );
  }

  const StatusTypeProvider = props => (
    <DataTypeProvider
      formatterComponent={StatusFormatter}
      {...props}
    />
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

  const sendRequest = (url, methodName, body, token) => {
    const Authorization = token !== null ? 'Bearer ' + token : null;
    return fetch(url, {
      method: methodName,
      headers: {'Content-Type':'application/json',
       Accept: 'application/json', 
       Authorization
      },
      body: body,
    }).catch(() => console.log('Error during request to server: method='+methodName+'.'));
  }

  const getRowId = (row) => row.id;

  const FAKE_TOKEN = "WEQWE//1asd2kgfg3434ldmdnbcvoireoir!q23--234j"

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
      token: props.role === 'Admin' ? FAKE_TOKEN : null
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

  getApiUrl = () => {
    return "/api/webpages";
  }

  getUpdateUrl = () => {
    return "/updateHub";
  }

  componentDidMount() {
    this.loadData();
    this.createHub()  
  }

  componentDidUpdate(prevProps, prevState) {
    this.loadData();
    if (this.props.role !== prevProps.role)
      this.setState({ token: this.props.role === 'Admin' ? FAKE_TOKEN : null});
  }

  getStateDeletingRows() {
    const { deletingRows } = this.state;
    return deletingRows;
  }

  getStateRows() {
    const { rows } = this.state;
    return rows;
  };

  hasToken = () => {
    return this.state.token !== null;
  }

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
    let { rows, token } = this.state;
    if (added) {
      const addedPage = JSON.stringify(added[0]);
      sendRequest(this.getApiUrl(), 'post', addedPage, token)
      .then(result => result.json())
      .then((data) => {
        rows = [...rows, data];
        this.setState({ rows });
      });        
    }
    else if (changed) {     
      const item = rows.filter(row => changed[row.id])[0];       
      const updatedPage = JSON.stringify({ ...item, ...changed[item.id] });
      sendRequest(this.getApiUrl(), 'post', updatedPage, token)
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
    const { token } = this.state;
    this.getStateDeletingRows().forEach((rowId) => {
      const index = rows.findIndex((row) => row.id === rowId);
      if (index > -1) {
        sendRequest(this.getApiUrl()+ '/' + rowId, 'delete', null, token)
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
        .withUrl(this.getUpdateUrl())
        .build();
    hubConnection.start()
        .catch(err => console.log('Error while establishing connection :('));
    hubConnection.on("UpdateStatus", (pages) => {
        this.setState({ rows: pages });
      });

    this.setState({ hubConnection });
  }

  loadData() {
    if (this.getApiUrl() === this.lastQuery) {
      this.setState({ loading: false });
      return;
    }
    sendRequest(this.getApiUrl(), 'GET', null, null)
    .then(result => result.json())
    .then((data) => {
        this.setState({
             rows: data,
             loading: false,
        });
    });
    this.lastQuery = this.getApiUrl();
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
          <StatusTypeProvider for={['status']} />
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
          {this.hasToken() && <TableEditColumn width={120}
            showAddCommand={!addedRows.length}
            showEditCommand
            showDeleteCommand
            commandComponent={Command} />}
        </Grid>
        <Dialog open={!!deletingRows.length}
          onClose={this.cancelDelete} >
          <DialogTitle>Delete record</DialogTitle>
          <DialogContent>
            <DialogContentText>Are you sure to delete the following record?</DialogContentText>
            <Paper>
              <Grid rows={rows.filter((row) => row.id === deletingRows[0])}
                columns={columns} >
                <StatusTypeProvider for={['status']} />
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

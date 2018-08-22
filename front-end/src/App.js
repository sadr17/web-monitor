import React, { Component } from 'react';
import './App.css';
import { WebPagesView } from './components/web-pages-view';
import Dropdown from 'react-dropdown'
import 'react-dropdown/style.css'

const DefaultOption = 'Anonym';

const options = [
  DefaultOption , 'Admin'
]

class App extends Component {

constructor(props) {
  super(props);
  this.state = {
    selectedRole: DefaultOption,
  };
}

onSelect(event) {
  console.log(event.value);
  this.setState({ selectedRole: event.value });
}

render() {
    return (
      <div className="App">
        <header className="App-header">
          <div className='Header-container'>
           <h1 className="App-title">Web pages monitor</h1>
           <Dropdown options={options} onChange={(e) => this.onSelect(e)} value={DefaultOption} placeholder="Select a role" />
          </div>
        </header>
        <div className="App-intro">
          <WebPagesView role={this.state.selectedRole} />
        </div>
      </div>
    );
  }
}

export default App;

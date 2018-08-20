import React, { Component } from 'react';
import './App.css';
import { WebPagesView } from './components/web-pages-view';

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <h1 className="App-title">Web pages monitor</h1>
        </header>
        <div className="App-intro">
          <WebPagesView />
        </div>
      </div>
    );
  }
}

export default App;

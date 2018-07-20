import React, { Component } from 'react';
import './App.css';
import axios from 'axios';

class App extends Component {
  state = {
    values: []
  }

  componentDidMount() {
    axios.get('/api/values')
    .then(response => {
      console.log(response.data);
      this.setState({
        values: response.data
      })
    })
    .catch(error => {
      console.log(error);
    })
  }

  render() {
    const values = this.state.values.map(value => (
      <li>{value.id} : {value.name}</li>
    ))
    return (
      <div className="App">
        <ul>
          {values}
        </ul>
      </div>
    );
  }
}

export default App;

import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios'
import Dropzone from 'react-dropzone'

class App extends Component {
    constructor() {
        super()
        this.state = { 
          files: [] 
        }
      }
    
      onDrop(files) {
        let formData = new FormData();
        formData.append("File", files);

        axios({
          method: 'post',
          url: '/api/users/1/photos',
          data: formData,
          config: { headers: {'Content-Type': 'multipart/form-data' }}
          })
          .then((response) => {
              console.log(response.data);
          })
          .catch((response) => {
              console.log(response);
          });
      }
    
      render() {
        return (
          <section>
            <div className="dropzone">
              <Dropzone onDrop={this.onDrop.bind(this)}>
                <p>Try dropping some files here, or click to select files to upload.</p>
              </Dropzone>
            </div>
            <aside>
              <h2>Dropped files</h2>
              <ul>
                {
                  this.state.files.map(f => <li key={f.name}>{f.name} - {f.size} bytes</li>)
                }
              </ul>
            </aside>
          </section>
        );
      }
}

export default App;

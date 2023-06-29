import './App.css'
import Test from './components/Test.jsx'

export default function App() {
    const addOrEdit = (formData, onSuccess) => {
        
    }
  return (
    <>
      <h1>Gallery App</h1>
      <p>
        Click on the Vite and React logos to learn more
      </p>
      <Test addOrEdit={addOrEdit}></Test>
    </>
  )
}

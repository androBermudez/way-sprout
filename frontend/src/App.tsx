import { BrowserRouter, Route, Routes } from "react-router-dom"

import { ApplicationDetailsPage } from "@/pages/ApplicationDetailsPage"
import { ApplicationFormPage } from "@/pages/ApplicationFormPage"
import { ApplicationsLayout } from "@/pages/ApplicationsLayout"
import { ApplicationsPage } from "@/pages/ApplicationsPage"
import { HomePage } from "@/pages/HomePage"

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} index={true} />
        <Route path="/applications" element={<ApplicationsLayout />}>
          <Route index element={<ApplicationsPage />} />
          <Route path="new" element={<ApplicationFormPage />} />
          <Route path=":id" element={<ApplicationDetailsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App

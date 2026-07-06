import { useNavigate } from "react-router-dom"
import { useQuery } from "@tanstack/react-query"

import { getJobApplications } from "@/api/jobApplication"
import { Badge } from "@/components/ui/badge"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

export function ApplicationsPage() {
  const navigate = useNavigate()
  const { data, isLoading, isError } = useQuery({
    queryKey: ["job-applications"],
    queryFn: getJobApplications,
  })

  if (isLoading) return <p className="p-6">Loading...</p>
  if (isError)
    return <p className="p-6 text-destructive">Failed to load applications.</p>

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Company</TableHead>
          <TableHead>Position</TableHead>
          <TableHead>Status</TableHead>
          <TableHead>Applied on</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {data?.map((application) => (
          <TableRow
            key={application.id}
            className="cursor-pointer"
            onClick={() => navigate(`/applications/${application.id}`)}
          >
            <TableCell>{application.company}</TableCell>
            <TableCell>{application.position}</TableCell>
            <TableCell>
              <Badge>{application.status}</Badge>
            </TableCell>
            <TableCell>{application.appliedOn}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}

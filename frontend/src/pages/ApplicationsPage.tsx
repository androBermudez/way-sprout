import { useQuery } from "@tanstack/react-query";
import { getJobApplications } from "@/api/jobApplication";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export function ApplicationsPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["job-applications"],
    queryFn: getJobApplications,
  });

  if (isLoading) return <p className="p-6">Cargando...</p>;
  if (isError)
    return (
      <p className="p-6 text-destructive">Error al cargar las postulaciones.</p>
    );

  return (
    <div className="p-6">
      <h1 className="text-2xl font-semibold mb-4">Postulaciones</h1>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Empresa</TableHead>
            <TableHead>Posición</TableHead>
            <TableHead>Estado</TableHead>
            <TableHead>Aplicado el</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {data?.map((application) => (
            <TableRow key={application.id}>
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
    </div>
  );
}

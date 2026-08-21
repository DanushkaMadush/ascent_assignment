import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Snackbar,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";

import {
  Add,
  Close,
  Delete,
  Edit,
  Search,
} from "@mui/icons-material";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";

import { employeeService } from "../../api/services/employeeService";
import type {
  Employee,
  UpdateEmployeeRequest,
} from "../../api/models/employee";

const PAGE_SIZE = 10;

const departments = [
  { id: 1, name: "IT" },
  { id: 2, name: "Human Resources" },
  { id: 3, name: "Finance" },
  { id: 4, name: "Sales" },
];

const managers = [
  { id: "manager-1", name: "John Manager" },
  { id: "manager-2", name: "Sarah Manager" },
  { id: "manager-3", name: "David Manager" },
];

const roles = [
  "Administrator",
  "Manager",
  "Developer",
  "HR",
  "Accountant",
  "Sales",
];

interface SnackbarState {
  open: boolean;
  message: string;
  severity: "success" | "error";
}

const EmployeePage = () => {
  const navigate = useNavigate();

  const [employees, setEmployees] = useState<Employee[]>([]);

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(PAGE_SIZE);
  const [totalRecords, setTotalRecords] = useState(0);

  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(false);

  const [selectedEmployee, setSelectedEmployee] =
    useState<Employee | null>(null);

  const [editForm, setEditForm] =
    useState<UpdateEmployeeRequest | null>(null);

  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  const [snackbar, setSnackbar] = useState<SnackbarState>({
    open: false,
    message: "",
    severity: "success",
  });

  /*
   * Debounce search input.
   */
  useEffect(() => {
    const timer = window.setTimeout(() => {
      setSearch(searchInput);
      setPage(0);
    }, 500);

    return () => {
      window.clearTimeout(timer);
    };
  }, [searchInput]);

  /*
   * Load employees.
   */
  const loadEmployees = useCallback(async () => {
    try {
      setLoading(true);

      const response = await employeeService.getAll({
        PageNumber: page + 1,
        PageSize: pageSize,
        search: search.trim() || undefined,
      });

      if (!response.success) {
        throw new Error(response.message || "Failed to load employees.");
      }

      setEmployees(response.data.data);
      setTotalRecords(response.data.pagination.totalRecords);
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : "Failed to load employees.";

      setSnackbar({
        open: true,
        message,
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, search]);

  useEffect(() => {
    loadEmployees();
  }, [loadEmployees]);

  /*
   * Open employee details/edit modal.
   */
  const handleRowClick = async (employee: Employee) => {
    try {
      setSelectedEmployee(employee);

      /*
       * Fetch latest employee details.
       */
      const response = await employeeService.getById(employee.id);

      if (!response.success) {
        throw new Error(
          response.message || "Failed to load employee details."
        );
      }

      const data = response.data;

      setSelectedEmployee(data);

      setEditForm({
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        phoneNumber: data.phoneNumber,
        departmentId: data.departmentId,
        managerId: data.managerId,
        role: data.role,
        isActive: data.isActive,
      });
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : "Failed to load employee details.";

      setSelectedEmployee(null);

      setSnackbar({
        open: true,
        message,
        severity: "error",
      });
    }
  };

  const handleCloseDetails = () => {
    if (saving || deleting) {
      return;
    }

    setSelectedEmployee(null);
    setEditForm(null);
  };

  const handleEditChange = <K extends keyof UpdateEmployeeRequest>(
    field: K,
    value: UpdateEmployeeRequest[K]
  ) => {
    setEditForm((previous) => {
      if (!previous) {
        return previous;
      }

      return {
        ...previous,
        [field]: value,
      };
    });
  };

  /*
   * Update employee.
   */
  const handleUpdate = async () => {
    if (!selectedEmployee || !editForm) {
      return;
    }

    try {
      setSaving(true);

      const response = await employeeService.update(
        selectedEmployee.id,
        editForm
      );

      if (!response.success) {
        throw new Error(response.message || "Failed to update employee.");
      }

      setSnackbar({
        open: true,
        message: response.message || "Employee updated successfully.",
        severity: "success",
      });

      setSelectedEmployee(null);
      setEditForm(null);

      await loadEmployees();
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : "Failed to update employee.";

      setSnackbar({
        open: true,
        message,
        severity: "error",
      });
    } finally {
      setSaving(false);
    }
  };

  /*
   * Delete employee.
   */
  const handleDelete = async () => {
    if (!selectedEmployee) {
      return;
    }

    try {
      setDeleting(true);

      const response = await employeeService.remove(
        selectedEmployee.id
      );

      if (!response.success) {
        throw new Error(response.message || "Failed to delete employee.");
      }

      setDeleteDialogOpen(false);
      setSelectedEmployee(null);
      setEditForm(null);

      setSnackbar({
        open: true,
        message: response.message || "Employee deleted successfully.",
        severity: "success",
      });

      /*
       * If the current page becomes empty after deletion,
       * move back one page.
       */
      if (employees.length === 1 && page > 0) {
        setPage((previous) => previous - 1);
      } else {
        await loadEmployees();
      }
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : "Failed to delete employee.";

      setSnackbar({
        open: true,
        message,
        severity: "error",
      });
    } finally {
      setDeleting(false);
    }
  };

  const handleChangePage = (
    _: unknown,
    newPage: number
  ) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setPageSize(Number(event.target.value));
    setPage(0);
  };

  const departmentName = useMemo(() => {
    if (!editForm) {
      return "";
    }

    return (
      departments.find(
        (department) => department.id === editForm.departmentId
      )?.name ?? ""
    );
  }, [editForm]);

  return (
    <Box sx={{ margin: 15 }}>
      {/* Header */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          mb: 3,
        }}
      >
        <Typography variant="h4" sx={{ fontWeight: 600 }}>
          Employee Management
        </Typography>
      </Box>

      {/* Search + Add */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          gap: 2,
          mb: 2,
        }}
      >
        <TextField
          value={searchInput}
          onChange={(event) => setSearchInput(event.target.value)}
          placeholder="Search employees..."
          size="small"
          sx={{
            width: 350,
          }}
          slotProps={{
            input: {
              startAdornment: (
                <Search
                  fontSize="small"
                  sx={{ mr: 1, color: "text.secondary" }}
                />
              ),
            },
          }}
        />

        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => navigate("/employees/new")}
        >
          Add New
        </Button>
      </Box>

      {/* Table */}
      <Paper variant="outlined">
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>
                  <strong>Employee Code</strong>
                </TableCell>

                <TableCell>
                  <strong>Name</strong>
                </TableCell>

                <TableCell>
                  <strong>Email</strong>
                </TableCell>

                <TableCell>
                  <strong>Department</strong>
                </TableCell>

                <TableCell>
                  <strong>Role</strong>
                </TableCell>

                <TableCell>
                  <strong>Status</strong>
                </TableCell>

                <TableCell align="right">
                  <strong>Actions</strong>
                </TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={7}>
                    <Box
                      sx={{
                        display: "flex",
                        justifyContent: "center",
                        py: 5,
                      }}
                    >
                      <CircularProgress />
                    </Box>
                  </TableCell>
                </TableRow>
              ) : employees.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7}>
                    <Box
                      sx={{
                        textAlign: "center",
                        py: 5,
                      }}
                    >
                      <Typography color="text.secondary">
                        No employees found.
                      </Typography>
                    </Box>
                  </TableCell>
                </TableRow>
              ) : (
                employees.map((employee) => (
                  <TableRow
                    key={employee.id}
                    hover
                    sx={{
                      cursor: "pointer",
                    }}
                    onClick={() => handleRowClick(employee)}
                  >
                    <TableCell>
                      {employee.employeeCode}
                    </TableCell>

                    <TableCell>
                      {employee.firstName} {employee.lastName}
                    </TableCell>

                    <TableCell>
                      {employee.email}
                    </TableCell>

                    <TableCell>
                      {employee.departmentName}
                    </TableCell>

                    <TableCell>
                      {employee.role}
                    </TableCell>

                    <TableCell>
                      <Chip
                        size="small"
                        label={
                          employee.isActive
                            ? "Active"
                            : "Inactive"
                        }
                        color={
                          employee.isActive
                            ? "success"
                            : "default"
                        }
                      />
                    </TableCell>

                    <TableCell
                      align="right"
                      onClick={(event) => {
                        event.stopPropagation();
                      }}
                    >
                      <IconButton
                        size="small"
                        onClick={() => handleRowClick(employee)}
                        title="Edit"
                      >
                        <Edit fontSize="small" />
                      </IconButton>

                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => {
                          setSelectedEmployee(employee);
                          setDeleteDialogOpen(true);
                        }}
                        title="Delete"
                      >
                        <Delete fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>

        <TablePagination
          component="div"
          count={totalRecords}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={pageSize}
          onRowsPerPageChange={handleChangeRowsPerPage}
          rowsPerPageOptions={[5, 10, 25, 50]}
        />
      </Paper>

      {/* Employee Edit Dialog */}
      <Dialog
        open={Boolean(selectedEmployee && editForm)}
        onClose={handleCloseDetails}
        fullWidth
        maxWidth="md"
      >
        <DialogTitle
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          Edit Employee

          <IconButton
            onClick={handleCloseDetails}
            disabled={saving || deleting}
          >
            <Close />
          </IconButton>
        </DialogTitle>

        <DialogContent dividers>
          {editForm && (
            <Box
              sx={{
                display: "grid",
                gridTemplateColumns: {
                  xs: "1fr",
                  sm: "1fr 1fr",
                },
                gap: 2,
                pt: 1,
              }}
            >
              <TextField
                label="First Name"
                value={editForm.firstName}
                onChange={(event) =>
                  handleEditChange(
                    "firstName",
                    event.target.value
                  )
                }
                fullWidth
              />

              <TextField
                label="Last Name"
                value={editForm.lastName}
                onChange={(event) =>
                  handleEditChange(
                    "lastName",
                    event.target.value
                  )
                }
                fullWidth
              />

              <TextField
                label="Email"
                type="email"
                value={editForm.email}
                onChange={(event) =>
                  handleEditChange(
                    "email",
                    event.target.value
                  )
                }
                fullWidth
              />

              <TextField
                label="Phone Number"
                value={editForm.phoneNumber}
                onChange={(event) =>
                  handleEditChange(
                    "phoneNumber",
                    event.target.value
                  )
                }
                fullWidth
              />

              <TextField
                select
                label="Department"
                value={editForm.departmentId}
                onChange={(event) =>
                  handleEditChange(
                    "departmentId",
                    Number(event.target.value)
                  )
                }
                fullWidth
                slotProps={{
                  select: {
                    native: true,
                  },
                }}
              >
                <option value="">
                  Select Department
                </option>

                {departments.map((department) => (
                  <option
                    key={department.id}
                    value={department.id}
                  >
                    {department.name}
                  </option>
                ))}
              </TextField>

              <TextField
                label="Department Name"
                value={departmentName}
                disabled
                fullWidth
              />

              <TextField
                select
                value={editForm.managerId}
                onChange={(event) =>
                  handleEditChange(
                    "managerId",
                    event.target.value
                  )
                }
                fullWidth
                slotProps={{
                  select: {
                    native: true,
                  },
                }}
              >
                <option value="">
                  Select Manager
                </option>

                {managers.map((manager) => (
                  <option
                    key={manager.id}
                    value={manager.id}
                  >
                    {manager.name}
                  </option>
                ))}
              </TextField>

              <TextField
                select
                value={editForm.role}
                onChange={(event) =>
                  handleEditChange(
                    "role",
                    event.target.value
                  )
                }
                fullWidth
                slotProps={{
                  select: {
                    native: true,
                  },
                }}
              >
                <option value="">
                  Select Role
                </option>

                {roles.map((role) => (
                  <option key={role} value={role}>
                    {role}
                  </option>
                ))}
              </TextField>

              <TextField
                select
                label="Status"
                value={editForm.isActive ? "true" : "false"}
                onChange={(event) =>
                  handleEditChange(
                    "isActive",
                    event.target.value === "true"
                  )
                }
                fullWidth
                slotProps={{
                  select: {
                    native: true,
                  },
                }}
              >
                <option value="true">
                  Active
                </option>

                <option value="false">
                  Inactive
                </option>
              </TextField>

              {selectedEmployee && (
                <>
                  <TextField
                    label="Employee Code"
                    value={selectedEmployee.employeeCode}
                    disabled
                    fullWidth
                  />

                  <TextField
                    label="Created At"
                    value={new Date(
                      selectedEmployee.createdAt
                    ).toLocaleString()}
                    disabled
                    fullWidth
                  />
                </>
              )}
            </Box>
          )}
        </DialogContent>

        <DialogActions
          sx={{
            justifyContent: "space-between",
            px: 3,
            py: 2,
          }}
        >
          <Button
            color="error"
            startIcon={<Delete />}
            onClick={() => setDeleteDialogOpen(true)}
            disabled={saving || deleting}
          >
            Delete
          </Button>

          <Box sx={{ display: "flex", gap: 1 }}>
            <Button
              onClick={handleCloseDetails}
              disabled={saving || deleting}
            >
              Cancel
            </Button>

            <Button
              variant="contained"
              startIcon={
                saving ? (
                  <CircularProgress size={18} />
                ) : (
                  <Edit />
                )
              }
              onClick={handleUpdate}
              disabled={saving || deleting}
            >
              {saving ? "Updating..." : "Update"}
            </Button>
          </Box>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <Dialog
        open={deleteDialogOpen}
        onClose={() => {
          if (!deleting) {
            setDeleteDialogOpen(false);
          }
        }}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>
          Delete Employee
        </DialogTitle>

        <DialogContent>
          <Typography>
            Are you sure you want to delete{" "}
            <strong>
              {selectedEmployee?.firstName}{" "}
              {selectedEmployee?.lastName}
            </strong>
            ?
          </Typography>

          <Typography
            color="text.secondary"
            variant="body2"
            sx={{ mt: 1 }}
          >
            This action cannot be undone.
          </Typography>
        </DialogContent>

        <DialogActions>
          <Button
            onClick={() => setDeleteDialogOpen(false)}
            disabled={deleting}
          >
            Cancel
          </Button>

          <Button
            color="error"
            variant="contained"
            onClick={handleDelete}
            disabled={deleting}
            startIcon={
              deleting ? (
                <CircularProgress size={18} />
              ) : (
                <Delete />
              )
            }
          >
            {deleting ? "Deleting..." : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Notifications */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() =>
          setSnackbar((previous) => ({
            ...previous,
            open: false,
          }))
        }
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "right",
        }}
      >
        <Alert
          severity={snackbar.severity}
          variant="filled"
          onClose={() =>
            setSnackbar((previous) => ({
              ...previous,
              open: false,
            }))
          }
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default EmployeePage;
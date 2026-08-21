import {
  Alert,
  Box,
  Button,
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

import { Add, Close, Edit, Search } from "@mui/icons-material";

import { useCallback, useEffect, useState } from "react";

import { useNavigate } from "react-router-dom";

import { departmentService } from "../../api/services/departmentService";

import type {
  Department,
  UpdateDepartmentRequest,
} from "../../api/models/department";

const PAGE_SIZE = 10;

interface SnackbarState {
  open: boolean;
  message: string;
  severity: "success" | "error";
}

const DepartmentPage = () => {
  const navigate = useNavigate();

  const [departments, setDepartments] = useState<Department[]>([]);

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(PAGE_SIZE);
  const [totalRecords, setTotalRecords] = useState(0);

  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(false);

  const [selectedDepartment, setSelectedDepartment] =
    useState<Department | null>(null);

  const [editForm, setEditForm] = useState<UpdateDepartmentRequest | null>(
    null,
  );

  const [saving, setSaving] = useState(false);

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
   * Load departments.
   */
  const loadDepartments = useCallback(async () => {
    try {
      setLoading(true);

      const response = await departmentService.getAll({
        pageNumber: page + 1,
        pageSize,
        search: search.trim() || undefined,
      });

      if (!response.success) {
        throw new Error(response.message || "Failed to load departments.");
      }

      setDepartments(response.data.data);
      setTotalRecords(response.data.pagination.totalRecords);
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Failed to load departments.";

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
    void loadDepartments();
  }, [loadDepartments]);

  /*
   * Open department details/edit modal.
   */
  const handleRowClick = async (department: Department) => {
    try {
      /*
       * Show the selected department immediately while
       * the latest details are being fetched.
       */
      setSelectedDepartment(department);

      setEditForm({
        name: department.name,
        description: department.description,
      });

      /*
       * Fetch latest department details.
       */
      const response = await departmentService.getById(department.id);

      if (!response.success) {
        throw new Error(
          response.message || "Failed to load department details.",
        );
      }

      const data = response.data;

      setSelectedDepartment(data);

      setEditForm({
        name: data.name,
        description: data.description,
      });
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : "Failed to load department details.";

      setSelectedDepartment(null);
      setEditForm(null);

      setSnackbar({
        open: true,
        message,
        severity: "error",
      });
    }
  };

  /*
   * Close details/edit modal.
   */
  const handleCloseDetails = () => {
    if (saving) {
      return;
    }

    setSelectedDepartment(null);
    setEditForm(null);
  };

  /*
   * Change edit form.
   */
  const handleEditChange = <K extends keyof UpdateDepartmentRequest>(
    field: K,
    value: UpdateDepartmentRequest[K],
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
   * Update department.
   */
  const handleUpdate = async () => {
    if (!selectedDepartment || !editForm) {
      return;
    }

    if (!editForm.name.trim()) {
      setSnackbar({
        open: true,
        message: "Department name is required.",
        severity: "error",
      });

      return;
    }

    if (!editForm.description.trim()) {
      setSnackbar({
        open: true,
        message: "Department description is required.",
        severity: "error",
      });

      return;
    }

    try {
      setSaving(true);

      const request: UpdateDepartmentRequest = {
        name: editForm.name.trim(),
        description: editForm.description.trim(),
      };

      const response = await departmentService.update(
        selectedDepartment.id,
        request,
      );

      if (!response.success) {
        throw new Error(response.message || "Failed to update department.");
      }

      setSnackbar({
        open: true,
        message: response.message || "Department updated successfully.",
        severity: "success",
      });

      setSelectedDepartment(null);
      setEditForm(null);

      await loadDepartments();
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Failed to update department.";

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
   * Pagination.
   */
  const handleChangePage = (_: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>,
  ) => {
    setPageSize(Number(event.target.value));
    setPage(0);
  };

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
          Department Management
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
          placeholder="Search departments..."
          size="small"
          sx={{
            width: 350,
          }}
          slotProps={{
            input: {
              startAdornment: (
                <Search
                  fontSize="small"
                  sx={{
                    mr: 1,
                    color: "text.secondary",
                  }}
                />
              ),
            },
          }}
        />

        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => navigate("/departments/new")}
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
                  <strong>Name</strong>
                </TableCell>

                <TableCell>
                  <strong>Description</strong>
                </TableCell>

                <TableCell>
                  <strong>Employee Count</strong>
                </TableCell>

                <TableCell align="right">
                  <strong>Actions</strong>
                </TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={4}>
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
              ) : departments.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4}>
                    <Box
                      sx={{
                        textAlign: "center",
                        py: 5,
                      }}
                    >
                      <Typography color="text.secondary">
                        No departments found.
                      </Typography>
                    </Box>
                  </TableCell>
                </TableRow>
              ) : (
                departments.map((department) => (
                  <TableRow
                    key={department.id}
                    hover
                    sx={{
                      cursor: "pointer",
                    }}
                    onClick={() => void handleRowClick(department)}
                  >
                    <TableCell>{department.name}</TableCell>

                    <TableCell>{department.description}</TableCell>

                    <TableCell>{department.employeeCount}</TableCell>

                    <TableCell
                      align="right"
                      onClick={(event) => {
                        event.stopPropagation();
                      }}
                    >
                      <IconButton
                        size="small"
                        onClick={() => void handleRowClick(department)}
                        title="Edit"
                      >
                        <Edit fontSize="small" />
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

      {/* Department Details/Edit Dialog */}
      <Dialog
        open={Boolean(selectedDepartment && editForm)}
        onClose={handleCloseDetails}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          Edit Department
          <IconButton onClick={handleCloseDetails} disabled={saving}>
            <Close />
          </IconButton>
        </DialogTitle>

        <DialogContent dividers>
          {editForm && selectedDepartment && (
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
                label="Department Name"
                value={editForm.name}
                onChange={(event) =>
                  handleEditChange("name", event.target.value)
                }
                fullWidth
              />

              <TextField
                label="Employee Count"
                value={selectedDepartment.employeeCount}
                disabled
                fullWidth
              />

              <TextField
                label="Description"
                value={editForm.description}
                onChange={(event) =>
                  handleEditChange("description", event.target.value)
                }
                multiline
                minRows={4}
                fullWidth
                sx={{
                  gridColumn: {
                    xs: "auto",
                    sm: "1 / -1",
                  },
                }}
              />
            </Box>
          )}
        </DialogContent>

        <DialogActions
          sx={{
            px: 3,
            py: 2,
          }}
        >
          <Button onClick={handleCloseDetails} disabled={saving}>
            Cancel
          </Button>

          <Button
            variant="contained"
            startIcon={saving ? <CircularProgress size={18} /> : <Edit />}
            onClick={() => void handleUpdate()}
            disabled={saving}
          >
            {saving ? "Updating..." : "Update"}
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

export default DepartmentPage;

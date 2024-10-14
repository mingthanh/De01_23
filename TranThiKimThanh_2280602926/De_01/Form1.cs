using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows.Forms;

namespace De_01
{
    public partial class frmQuanlysinhvien : Form
    {
        QuanlySVEntities you = new QuanlySVEntities();

        public frmQuanlysinhvien()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadComboBox();
        }

        private void frmQuanlysinhvien_Load(object sender, EventArgs e)
        {
            loadData();
        }

        void InitializeDataGridView()
        {
            dgvSinhvien.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn colMaSV = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaSV",
                Name = "MaSV",
                HeaderText = "Mã Sinh Viên",
                Width = 100
            };
            dgvSinhvien.Columns.Add(colMaSV);

            DataGridViewTextBoxColumn colHotenSV = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HotenSV",
                Name = "HotenSV",
                HeaderText = "Họ Tên",
                Width = 200
            };
            dgvSinhvien.Columns.Add(colHotenSV);

            DataGridViewTextBoxColumn colMaLop = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaLop",
                Name = "MaLop",
                HeaderText = "Mã Lớp",
                Width = 100
            };
            dgvSinhvien.Columns.Add(colMaLop);

            DataGridViewTextBoxColumn colNgaysinh = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ngaysinh",
                Name = "Ngaysinh",
                HeaderText = "Ngày Sinh",
                Width = 100
            };
            dgvSinhvien.Columns.Add(colNgaysinh);
        }

        void LoadComboBox()
        {
            try
            {
                var lops = you.Lops.ToList();
                cmblop.DataSource = lops;
                cmblop.DisplayMember = "TenLop";
                cmblop.ValueMember = "MaLop";
                cmblop.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu lớp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void loadData()
        {
            try
            {
                dgvSinhvien.DataSource = you.Sinhviens.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnfind_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtfind.Text.Trim().ToLower();
                var filteredSinhviens = you.Sinhviens
                    .Where(sv => sv.HotenSV.ToLower().Contains(searchTerm))
                    .ToList();
                dgvSinhvien.DataSource = filteredSinhviens;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmblop.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn một lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maSV = txtmssv.Text.Trim();
                string hoTen = txthoten.Text.Trim();
                string maLop = cmblop.SelectedValue.ToString();
                DateTime ngaySinh = dtpngaysinh.Value;

                if (string.IsNullOrWhiteSpace(maSV))
                {
                    MessageBox.Show("Mã sinh viên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(hoTen))
                {
                    MessageBox.Show("Họ tên sinh viên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (hoTen.Length > 50)
                {
                    MessageBox.Show("Họ tên sinh viên không được vượt quá 50 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (you.Sinhviens.Any(s => s.MaSV == maSV))
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại! Vui lòng nhập mã khác.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var sv = new Sinhvien
                {
                    MaSV = maSV,
                    HotenSV = hoTen,
                    MaLop = maLop,
                    Ngaysinh = ngaySinh
                };

                you.Sinhviens.Add(sv);
                you.SaveChanges();
                loadData();
                ClearInputFields();
                MessageBox.Show("Thêm sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("\n", ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"Thuộc tính: {x.PropertyName} - Lỗi: {x.ErrorMessage}"));

                MessageBox.Show($"Lỗi xác thực dữ liệu:\n{errorMessages}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSinhvien.SelectedRows.Count > 0)
                {
                    string maSV = dgvSinhvien.SelectedRows[0].Cells["MaSV"].Value.ToString();
                    Sinhvien sv = you.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (sv != null)
                    {
                        DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên có mã {maSV} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            you.Sinhviens.Remove(sv);
                            you.SaveChanges();
                            loadData();
                            MessageBox.Show("Xóa sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnupd_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSinhvien.SelectedRows.Count > 0)
                {
                    string maSV = dgvSinhvien.SelectedRows[0].Cells["MaSV"].Value.ToString();
                    Sinhvien sv = you.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (sv != null)
                    {
                        if (cmblop.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn một lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string hoTen = txthoten.Text.Trim();
                        string maLop = cmblop.SelectedValue.ToString();
                        DateTime ngaySinh = dtpngaysinh.Value;

                        if (string.IsNullOrWhiteSpace(hoTen))
                        {
                            MessageBox.Show("Họ tên sinh viên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (hoTen.Length > 50)
                        {
                            MessageBox.Show("Họ tên sinh viên không được vượt quá 50 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        sv.HotenSV = hoTen;
                        sv.MaLop = maLop;
                        sv.Ngaysinh = ngaySinh;

                        you.SaveChanges();
                        loadData();
                        ClearInputFields();
                        MessageBox.Show("Cập nhật sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("\n", ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"Thuộc tính: {x.PropertyName} - Lỗi: {x.ErrorMessage}"));

                MessageBox.Show($"Lỗi xác thực dữ liệu:\n{errorMessages}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            try
            {
                you.SaveChanges();
                MessageBox.Show("Dữ liệu đã được lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loadData();
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("\n", ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"Thuộc tính: {x.PropertyName} - Lỗi: {x.ErrorMessage}"));

                MessageBox.Show($"Lỗi xác thực dữ liệu:\n{errorMessages}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnunsave_Click(object sender, EventArgs e)
        {
            try
            {
                you = new QuanlySVEntities();
                loadData();
                MessageBox.Show("Thay đổi đã được hủy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hủy lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ClearInputFields()
        {
            txtmssv.Clear();
            txthoten.Clear();
            cmblop.SelectedIndex = -1;
            dtpngaysinh.Value = DateTime.Now;
        }

        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhvien.Rows[e.RowIndex];
                txtmssv.Text = row.Cells["MaSV"].Value.ToString();
                txthoten.Text = row.Cells["HotenSV"].Value.ToString();
                string maLop = row.Cells["MaLop"].Value.ToString();
                if (cmblop.Items.Cast<Lop>().Any(l => l.MaLop == maLop))
                {
                    cmblop.SelectedValue = maLop;
                }
                else
                {
                    cmblop.SelectedIndex = -1;
                }
                if (DateTime.TryParse(row.Cells["Ngaysinh"].Value.ToString(), out DateTime ngaySinh))
                {
                    dtpngaysinh.Value = ngaySinh;
                }
            }
        }
    }
}

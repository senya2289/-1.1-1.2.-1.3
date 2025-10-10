using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StudentManagementSystem
{
    // ��������� �������
    public interface IStudent
    {
        string FullName { get; set; }
        string StudentId { get; set; }
        int Course { get; set; }
        string Group { get; set; }
        double CalculateAverageGrade();
        string GetCourseInfo();
        string GetStudentInfo();
        void AddGrade(string subject, int grade);
        Dictionary<string, List<int>> Grades { get; }
    }

    // ������� ����� ��������
    public abstract class BaseStudent : IStudent
    {
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }
        public Dictionary<string, List<int>> Grades { get; protected set; }

        public BaseStudent(string fullName, string studentId, int course, string group)
        {
            FullName = fullName;
            StudentId = studentId;
            Course = course;
            Group = group;
            Grades = new Dictionary<string, List<int>>();
        }

        public abstract double CalculateAverageGrade();

        public virtual string GetCourseInfo()
        {
            return $"{Course} ����, ������ {Group}";
        }

        public virtual string GetStudentInfo()
        {
            return $"{FullName} (ID: {StudentId}) - {GetCourseInfo()} - ������� ����: {CalculateAverageGrade():F2}";
        }

        public void AddGrade(string subject, int grade)
        {
            if (grade < 1 || grade > 5)
                throw new ArgumentException("������ ������ ���� �� 1 �� 5");

            if (!Grades.ContainsKey(subject))
            {
                Grades[subject] = new List<int>();
            }
            Grades[subject].Add(grade);
        }

        protected double CalculateAverageForSubject(string subject)
        {
            if (Grades.ContainsKey(subject) && Grades[subject].Count > 0)
            {
                double sum = 0;
                foreach (var grade in Grades[subject])
                {
                    sum += grade;
                }
                return sum / Grades[subject].Count;
            }
            return 0;
        }
    }

    // ������� 1 �����
    public class FirstYearStudent : BaseStudent
    {
        public FirstYearStudent(string fullName, string studentId, string group)
            : base(fullName, studentId, 1, group)
        {
        }

        public override double CalculateAverageGrade()
        {
            if (Grades.Count == 0) return 0;

            double totalSum = 0;
            int totalCount = 0;

            foreach (var subject in Grades.Keys)
            {
                totalSum += CalculateAverageForSubject(subject) * GetSubjectWeight(subject);
                totalCount += Grades[subject].Count;
            }

            return totalCount > 0 ? totalSum / totalCount : 0;
        }

        private double GetSubjectWeight(string subject)
        {
            // ������� �������� ����� ����������� ���
            return 1.0;
        }

        public override string GetCourseInfo()
        {
            return $"1 ���� (�����������), ������ {Group} - �������� ������� ���������";
        }
    }

    // ������� 2-3 �����
    public class MiddleYearStudent : BaseStudent
    {
        public string Specialization { get; set; }

        public MiddleYearStudent(string fullName, string studentId, int course, string group, string specialization)
            : base(fullName, studentId, course, group)
        {
            if (course < 2 || course > 3)
                throw new ArgumentException("���� ������ ���� 2 ��� 3");
            Specialization = specialization;
        }

        public override double CalculateAverageGrade()
        {
            if (Grades.Count == 0) return 0;

            double totalSum = 0;
            int totalCount = 0;

            foreach (var subject in Grades.Keys)
            {
                double weight = GetSubjectWeight(subject);
                totalSum += CalculateAverageForSubject(subject) * weight * Grades[subject].Count;
                totalCount += Grades[subject].Count;
            }

            return totalCount > 0 ? totalSum / totalCount : 0;
        }

        private double GetSubjectWeight(string subject)
        {
            // ���������� �������� ����� ������� ���
            if (IsCoreSubject(subject))
                return 1.2;
            return 1.0;
        }

        private bool IsCoreSubject(string subject)
        {
            string[] coreSubjects = { "����������������", "���� ������", "���������", "����" };
            return Array.Exists(coreSubjects, s => s.Equals(subject, StringComparison.OrdinalIgnoreCase));
        }

        public override string GetCourseInfo()
        {
            return $"{Course} ���� (�����������), ������ {Group} - ������������: {Specialization}";
        }

        public override string GetStudentInfo()
        {
            return $"{base.GetStudentInfo()} - �������������: {Specialization}";
        }
    }

    // ������� 4 ����� (���������)
    public class FinalYearStudent : BaseStudent
    {
        public string ThesisTopic { get; set; }
        public bool HasInternship { get; set; }

        public FinalYearStudent(string fullName, string studentId, string group, string thesisTopic, bool hasInternship)
            : base(fullName, studentId, 4, group)
        {
            ThesisTopic = thesisTopic;
            HasInternship = hasInternship;
        }

        public override double CalculateAverageGrade()
        {
            if (Grades.Count == 0) return 0;

            double totalSum = 0;
            int totalCount = 0;
            int thesisBonus = 0;

            foreach (var subject in Grades.Keys)
            {
                double weight = GetSubjectWeight(subject);
                double subjectAverage = CalculateAverageForSubject(subject);

                if (subject.Equals("��������� ������", StringComparison.OrdinalIgnoreCase))
                {
                    thesisBonus = (int)(subjectAverage * 0.5); // ����� �� ������
                }

                totalSum += subjectAverage * weight * Grades[subject].Count;
                totalCount += Grades[subject].Count;
            }

            double average = totalCount > 0 ? (totalSum / totalCount) + thesisBonus : 0;
            return Math.Min(average, 5.0); // �� ������ 5.0
        }

        private double GetSubjectWeight(string subject)
        {
            // ��������� ������ � ���������� �������� ����� ������� ���
            if (subject.Equals("��������� ������", StringComparison.OrdinalIgnoreCase))
                return 2.0;
            else if (IsCoreSubject(subject))
                return 1.3;
            return 1.0;
        }

        private bool IsCoreSubject(string subject)
        {
            string[] coreSubjects = { "��������", "��������������", "������������" };
            return Array.Exists(coreSubjects, s => s.Equals(subject, StringComparison.OrdinalIgnoreCase));
        }

        public override string GetCourseInfo()
        {
            string internshipInfo = HasInternship ? "� ���������" : "��� ��������";
            return $"4 ���� (���������), ������ {Group} - ������: '{ThesisTopic}' ({internshipInfo})";
        }

        public override string GetStudentInfo()
        {
            return $"{base.GetStudentInfo()} - ���� �������: {ThesisTopic}";
        }
    }

    // ����������
    public class MastersStudent : BaseStudent
    {
        public string ResearchArea { get; set; }
        public bool IsResearcher { get; set; }

        public MastersStudent(string fullName, string studentId, string group, string researchArea, bool isResearcher)
            : base(fullName, studentId, 5, group) // 5 ���� ��� ������������
        {
            ResearchArea = researchArea;
            IsResearcher = isResearcher;
        }

        public override double CalculateAverageGrade()
        {
            if (Grades.Count == 0) return 0;

            double totalSum = 0;
            int totalCount = 0;
            double researchBonus = IsResearcher ? 0.3 : 0;

            foreach (var subject in Grades.Keys)
            {
                double weight = GetSubjectWeight(subject);
                totalSum += CalculateAverageForSubject(subject) * weight * Grades[subject].Count;
                totalCount += Grades[subject].Count;
            }

            double average = totalCount > 0 ? (totalSum / totalCount) + researchBonus : 0;
            return Math.Min(average, 5.0);
        }

        private double GetSubjectWeight(string subject)
        {
            if (IsResearchSubject(subject))
                return 1.5;
            else if (IsCoreSubject(subject))
                return 1.2;
            return 1.0;
        }

        private bool IsResearchSubject(string subject)
        {
            string[] researchSubjects = { "������� ������������", "�����������", "����������" };
            return Array.Exists(researchSubjects, s => s.Equals(subject, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsCoreSubject(string subject)
        {
            string[] coreSubjects = { "�����������", "������", "������" };
            return Array.Exists(coreSubjects, s => s.Equals(subject, StringComparison.OrdinalIgnoreCase));
        }

        public override string GetCourseInfo()
        {
            string researcherInfo = IsResearcher ? "������� ���������" : "����������";
            return $"������������ ({Course} ����), ������ {Group} - �����������: {ResearchArea} ({researcherInfo})";
        }

        public override string GetStudentInfo()
        {
            return $"{base.GetStudentInfo()} - ������� ������������: {ResearchArea}";
        }
    }

    // �������� ����� ����������
    public partial class Form1 : Form
    {
        private List<IStudent> students;
        private IStudent selectedStudent;

        public Form1()
        {
            InitializeComponent();
            students = new List<IStudent>();
            InitializeCourseComboBox();
            LoadSampleData();
            RefreshStudentsList();
        }

        private void InitializeCourseComboBox()
        {
            cmbStudentType.Items.Add("1 ���� (�����������)");
            cmbStudentType.Items.Add("2-3 ���� (�����������)");
            cmbStudentType.Items.Add("4 ���� (���������)");
            cmbStudentType.Items.Add("������������");
            cmbStudentType.SelectedIndex = 0;
        }

        private void LoadSampleData()
        {
            // ������� ���������
            var student1 = new FirstYearStudent("������ ���� ��������", "B2024001", "��-01");
            student1.AddGrade("����������", 5);
            student1.AddGrade("����������������", 4);
            student1.AddGrade("������", 3);
            students.Add(student1);

            var student2 = new MiddleYearStudent("������ ���� ��������", "B2023001", 2, "��-21", "���������� ��");
            student2.AddGrade("����������������", 5);
            student2.AddGrade("���� ������", 4);
            student2.AddGrade("���������", 5);
            students.Add(student2);

            var student3 = new FinalYearStudent("�������� ���� ���������", "B2021001", "��-41",
                "���������� ������� ���������� �����", true);
            student3.AddGrade("��������� ������", 5);
            student3.AddGrade("��������", 4);
            students.Add(student3);

            var student4 = new MastersStudent("������ ������� ����������", "M2024001", "��-01",
                "������������� ���������", true);
            student4.AddGrade("������� ������������", 5);
            student4.AddGrade("�����������", 4);
            students.Add(student4);
        }

        private void cmbStudentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInputFields();
        }

        private void UpdateInputFields()
        {
            lblExtra1.Visible = false;
            txtExtra1.Visible = false;
            lblExtra2.Visible = false;
            chkExtra1.Visible = false;

            string studentType = cmbStudentType.SelectedItem.ToString();

            switch (studentType)
            {
                case "1 ���� (�����������)":
                    // �������������� ���� �� �����
                    break;

                case "2-3 ���� (�����������)":
                    lblExtra1.Text = "�������������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    break;

                case "4 ���� (���������)":
                    lblExtra1.Text = "���� �������:";
                    lblExtra2.Text = "������ ��������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    chkExtra1.Visible = true;
                    chkExtra1.Text = "��";
                    break;

                case "������������":
                    lblExtra1.Text = "������� ������������:";
                    lblExtra2.Text = "������� ���������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    chkExtra1.Visible = true;
                    chkExtra1.Text = "��";
                    break;
            }
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            try
            {
                IStudent student = CreateStudentFromInput();
                if (student != null)
                {
                    students.Add(student);
                    RefreshStudentsList();
                    ClearInputFields();
                    MessageBox.Show("������� ������� ��������!", "�����",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IStudent CreateStudentFromInput()
        {
            string fullName = txtFullName.Text;
            string studentId = txtStudentId.Text;
            string group = txtGroup.Text;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(group))
                throw new ArgumentException("��� �������� ���� ������ ���� ���������");

            string studentType = cmbStudentType.SelectedItem.ToString();

            switch (studentType)
            {
                case "1 ���� (�����������)":
                    return new FirstYearStudent(fullName, studentId, group);

                case "2-3 ���� (�����������)":
                    string specialization = txtExtra1.Text;
                    int course = int.Parse(txtCourse.Text);
                    if (course < 2 || course > 3)
                        throw new ArgumentException("���� ������ ���� 2 ��� 3");
                    return new MiddleYearStudent(fullName, studentId, course, group, specialization);

                case "4 ���� (���������)":
                    string thesisTopic = txtExtra1.Text;
                    bool hasInternship = chkExtra1.Checked;
                    return new FinalYearStudent(fullName, studentId, group, thesisTopic, hasInternship);

                case "������������":
                    string researchArea = txtExtra1.Text;
                    bool isResearcher = chkExtra1.Checked;
                    return new MastersStudent(fullName, studentId, group, researchArea, isResearcher);

                default:
                    throw new ArgumentException("����������� ��� ��������");
            }
        }

        private void RefreshStudentsList()
        {
            lstStudents.Items.Clear();
            double totalAverage = 0;
            int studentCount = students.Count;

            foreach (var student in students)
            {
                lstStudents.Items.Add(student.GetStudentInfo());
                totalAverage += student.CalculateAverageGrade();
            }

            lblTotalStudents.Text = $"����� ���������: {studentCount}";
            lblAverageGrade.Text = studentCount > 0 ? $"������� ����: {totalAverage / studentCount:F2}" : "������� ����: 0.00";
        }

        private void ClearInputFields()
        {
            txtFullName.Clear();
            txtStudentId.Clear();
            txtGroup.Clear();
            txtCourse.Text = "1";
            txtExtra1.Clear();
            chkExtra1.Checked = false;
        }

        private void btnRemoveStudent_Click(object sender, EventArgs e)
        {
            if (lstStudents.SelectedIndex >= 0 && lstStudents.SelectedIndex < students.Count)
            {
                students.RemoveAt(lstStudents.SelectedIndex);
                RefreshStudentsList();
            }
            else
            {
                MessageBox.Show("�������� �������� ��� ��������", "��������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lstStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstStudents.SelectedIndex >= 0 && lstStudents.SelectedIndex < students.Count)
            {
                selectedStudent = students[lstStudents.SelectedIndex];
                DisplayStudentDetails(selectedStudent);
            }
        }

        private void DisplayStudentDetails(IStudent student)
        {
            txtDetails.Clear();
            txtDetails.AppendText($"=== ���������� � �������� ===\r\n");
            txtDetails.AppendText($"���: {student.FullName}\r\n");
            txtDetails.AppendText($"ID: {student.StudentId}\r\n");
            txtDetails.AppendText($"{student.GetCourseInfo()}\r\n");
            txtDetails.AppendText($"������� ����: {student.CalculateAverageGrade():F2}\r\n");
            txtDetails.AppendText($"\r\n=== ������ ===\r\n");

            if (student.Grades.Count > 0)
            {
                foreach (var subject in student.Grades.Keys)
                {
                    double subjectAverage = 0;
                    foreach (var grade in student.Grades[subject])
                    {
                        subjectAverage += grade;
                    }
                    subjectAverage /= student.Grades[subject].Count;

                    txtDetails.AppendText($"{subject}: ");
                    foreach (var grade in student.Grades[subject])
                    {
                        txtDetails.AppendText($"{grade} ");
                    }
                    txtDetails.AppendText($"(�������: {subjectAverage:F2})\r\n");
                }
            }
            else
            {
                txtDetails.AppendText("������ �����������\r\n");
            }
        }

        private void btnAddGrade_Click(object sender, EventArgs e)
        {
            if (selectedStudent == null)
            {
                MessageBox.Show("�������� �������� ��� ���������� ������", "��������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string subject = txtSubject.Text;
            if (string.IsNullOrWhiteSpace(subject))
            {
                MessageBox.Show("������� �������� ��������", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int grade = int.Parse(txtGrade.Text);
                selectedStudent.AddGrade(subject, grade);
                RefreshStudentsList();
                DisplayStudentDetails(selectedStudent);
                txtSubject.Clear();
                txtGrade.Clear();
                MessageBox.Show("������ ���������!", "�����",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������: {ex.Message}", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                RefreshStudentsList();
                return;
            }

            lstStudents.Items.Clear();
            foreach (var student in students)
            {
                if (student.FullName.ToLower().Contains(searchTerm) ||
                    student.StudentId.ToLower().Contains(searchTerm) ||
                    student.Group.ToLower().Contains(searchTerm))
                {
                    lstStudents.Items.Add(student.GetStudentInfo());
                }
            }
        }

        private void btnShowTopStudents_Click(object sender, EventArgs e)
        {
            var sortedStudents = new List<IStudent>(students);
            sortedStudents.Sort((s1, s2) => s2.CalculateAverageGrade().CompareTo(s1.CalculateAverageGrade()));

            lstStudents.Items.Clear();
            for (int i = 0; i < Math.Min(5, sortedStudents.Count); i++)
            {
                lstStudents.Items.Add($"{i + 1}. {sortedStudents[i].GetStudentInfo()}");
            }
        }

        private void btnShowByCourse_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtCourseFilter.Text, out int course) && course >= 1 && course <= 5)
            {
                lstStudents.Items.Clear();
                foreach (var student in students)
                {
                    if (student.Course == course)
                    {
                        lstStudents.Items.Add(student.GetStudentInfo());
                    }
                }
            }
            else
            {
                MessageBox.Show("������� ���������� ����� ����� (1-5)", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
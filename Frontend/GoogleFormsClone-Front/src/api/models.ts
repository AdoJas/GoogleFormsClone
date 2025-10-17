// User
export interface UserPreferences {
    emailNotifications: boolean;
    theme: 'light' | 'dark' | 'auto';
    language: string;
}

export interface UserStats {
    formsCreated: number;
    responsesReceived: number;
    totalStorageUsed: number;
}

export interface User {
    id: string;
    email: string;
    name: string;
    avatarUrl?: string | null;
    role: 'user' | 'admin';
    isActive: boolean;
    preferences: UserPreferences;
    stats: UserStats;
    createdAt: string;
    updatedAt: string;
}

// Auth
export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    user: {
        id: string;
        email: string;
        name: string;
        role: string;
        avatarUrl: string;
    };
    expiresAt: string;
}

// Form
export interface QuestionOption {
    id: string;
    text: string;
    orderIndex: number;
    allowsCustomText: boolean;
}

export interface LinearScale {
    minValue: number;
    maxValue: number;
    minLabel: string;
    maxLabel: string;
}

export interface QuestionValidation {
    minLength?: number;
    maxLength?: number;
    pattern?: string;
    minValue?: number;
    maxValue?: number;
    fileTypes?: string[];
    maxFileSize?: number;
    maxFileCount?: number;
}

export interface QuestionAppearance {
    placeholder?: string;
    helpText?: string;
    imageUrl?: string;
}

export interface QuestionLogic {
    dependsOn?: string;
    condition?: string;
    value?: string;
    action?: string;
}

export interface Question {
    id: string;
    type: string;
    questionText: string;
    description: string;
    required: boolean;
    allowMultipleSelection: boolean;
    options: QuestionOption[];
    linearScale?: LinearScale;
    validation?: QuestionValidation;
    orderIndex: number;
    logic?: QuestionLogic;
    appearance?: QuestionAppearance;
}

export interface FormSettings {
    allowEditing: boolean;
    oneResponsePerUser: boolean;
    showProgress: boolean;
    confirmationMessage: string;
    collectEmails: boolean;
    allowResponseEditing: boolean;
    responseEditingDuration: number;
}

export interface AccessControl {
    isPublic: boolean;
    requirePassword: boolean;
    accessPassword: string;
}

export interface Form {
    id: string;
    createdBy: string;
    title: string;
    description: string;
    settings: FormSettings;
    isActive: boolean;
    accessControl: AccessControl;
    version: number;
    questions: Question[];
    createdAt: string;
    updatedAt: string;
}

// File
export interface FileResource {
    id: string;
    originalName: string;
    fileType: string;
    fileSize: number;
    uploadUrl: string;
}

// Response
export interface FileUpload {
    fileUrl: string;
    fileName: string;
    fileSize: number;
    mimeType: string;
}

export interface Answer {
    questionId: string;
    answerText?: string;
    selectedOptions?: string[];
    linearScaleValue?: number;
    fileUpload?: FileUpload;
}

export interface Response {
    id: string;
    formId: string;
    submittedBy?: string;
    submittedAt: string;
    answers: Answer[];
}

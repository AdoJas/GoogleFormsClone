<template>
  <div>
    <v-card outlined>
      <v-card-title>
        {{ title || 'Files' }}
        <v-spacer></v-spacer>
        <v-btn color="primary" small @click="triggerUpload">
          Upload
        </v-btn>
        <input
            ref="fileInput"
            type="file"
            class="d-none"
            @change="handleFileSelected"
        />
      </v-card-title>

      <v-divider></v-divider>

      <v-list v-if="files.length">
        <v-list-item v-for="file in files" :key="file.id">
          <v-list-item-content>
            <v-list-item-title>{{ file.name }}</v-list-item-title>
          </v-list-item-content>

          <v-list-item-action>
            <v-btn
                v-if="file.canDownload !== false"
                icon
                @click="downloadFile(file)"
            >
              <v-icon>mdi-download</v-icon>
            </v-btn>

            <v-btn icon @click="deleteFile(file)">
              <v-icon color="red">mdi-delete</v-icon>
            </v-btn>
          </v-list-item-action>
        </v-list-item>
      </v-list>

      <v-card-text v-else class="text-center">
        No files uploaded yet.
      </v-card-text>
    </v-card>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, watch, onMounted } from 'vue';
import { fileApi } from '@/api/fileApi';
import type { FileResource } from '@/api/models';

export default defineComponent({
  name: 'FileManager',
  props: {
    entityType: {
      type: String as () => 'User' | 'Form' | 'Question',
      required: true,
    },
    entityId: { type: String, required: true },
    title: { type: String, default: '' },
  },
  setup(props) {
    const files = ref<FileResource[]>([]);
    const fileInput = ref<HTMLInputElement | null>(null);

    const fetchFiles = async () => {
      files.value = await fileApi.getFilesByAssociatedEntity(
          props.entityId,
          props.entityType
      );
    };

    const triggerUpload = () => {
      fileInput.value?.click();
    };

    const handleFileSelected = async (event: Event) => {
      const target = event.target as HTMLInputElement;
      if (!target.files?.length) return;

      const file = target.files[0];
      try {
        await fileApi.uploadFile(file, props.entityId, props.entityType);
        await fetchFiles();
      } catch (err) {
        console.error('File upload failed', err);
      } finally {
        target.value = ''; // reset input
      }
    };

    const downloadFile = async (file: FileResource) => {
      try {
        const blob = await fileApi.downloadFile(file.id);
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = file.name;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
      } catch (err) {
        console.error('Download failed', err);
      }
    };

    const deleteFile = async (file: FileResource) => {
      if (!confirm(`Delete file "${file.name}"?`)) return;
      try {
        await fileApi.deleteFile(file.id);
        await fetchFiles();
      } catch (err) {
        console.error('Delete failed', err);
      }
    };

    onMounted(fetchFiles);
    watch([() => props.entityId, () => props.entityType], fetchFiles);

    return { files, fileInput, triggerUpload, handleFileSelected, downloadFile, deleteFile };
  },
});
</script>

<style scoped>
.d-none {
  display: none;
}
</style>

<script setup lang="ts">
import { ref, type Ref } from 'vue';
import { ImageSize, type IImageRequest } from '@/types';

const prompt = ref('');
const imgSrc: Ref<Blob> = ref(new Blob());
const imageUrl = ref('');

const generateImage = async (prompt: string) => {

    const request: IImageRequest = {
        prompt: prompt,
        size: ImageSize.Large,
    }

    const response = await fetch('/api/Image/RequestImage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    });
    imgSrc.value = await response.blob();
    imageUrl.value = URL.createObjectURL(imgSrc.value);
    console.log(imgSrc.value);
};
</script>
<template>
    <textarea v-model="prompt" placeholder="Enter a prompt"></textarea>
    <img :src="imageUrl" alt="">
    <button @click="generateImage(prompt)">
        Generate
    </button>
</template>